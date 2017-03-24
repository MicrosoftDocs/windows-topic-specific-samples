/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

using QuizGame.Common;
using QuizGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace QuizGame.ViewModel
{
    public class HostViewModel : BindableBase
    {
        public IGame Game { get; private set; }

        private IHostCommunicator HostCommunicator { get; set; }

        public HostViewModel(IGame game, IHostCommunicator hostCommunicator)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (hostCommunicator == null) throw new ArgumentNullException("hostCommunicator");

            this.Game = game;
            this.HostCommunicator = hostCommunicator;

            this.Game.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName.Equals("SubmittedAnswers"))
                {
                    this.OnPropertyChanged(() => this.PlayerProgress);
                }
                if (e.PropertyName.Equals("CurrentQuestion") &&
                    this.Game.Questions.Last() == this.Game.CurrentQuestion)
                {
                    this.NextButtonText = "Show results";
                }
            };
            this.Game.NewQuestionAvailable += (s, e) => this.HostCommunicator.SendQuestion(e.Question);
            this.NextButtonText = "Next question";

            Func<DispatchedHandler, Task> callOnUiThread = async (handler) => await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, handler);

            this.HostCommunicator.PlayerJoined += async (s, e) =>
                await callOnUiThread(() => this.Game.AddPlayer(e.PlayerName));
            this.HostCommunicator.PlayerDeparted += async (s, e) =>
                await callOnUiThread(() => this.Game.RemovePlayer(e.PlayerName));
            this.HostCommunicator.AnswerReceived += async (s, e) =>
                await callOnUiThread(() => this.Game.SubmitAnswer(e.PlayerName, e.AnswerIndex));

            this.HostCommunicator.EnterLobby();
        }

        public DelegateCommand StartGameCommand
        {
            get
            {
                return this.startGameCommand ?? (this.startGameCommand = new DelegateCommand(
                    () => {
                        this.Game.StartGame();
                        this.OnQuestionChanged();
                        this.GameState = GameState.GameUnderway;
                    }, 
                    () => this.GameState == GameState.Lobby));
            }
        }
        private DelegateCommand startGameCommand;

        public DelegateCommand NextCommand
        {
            get
            {
                return this.nextCommand ?? (this.nextCommand = new DelegateCommand(
                    () => {
                        this.Game.NextQuestion();
                        this.OnQuestionChanged();
                        if (this.Game.IsGameOver) this.GameState = GameState.Results;
                    },
                    () => this.GameState == GameState.GameUnderway && this.Game.CurrentQuestion != null));
            }
        }
        private DelegateCommand nextCommand;

        public DelegateCommand EndGameCommand
        {
			get { return this.endGameCommand ?? (this.endGameCommand = new DelegateCommand(
                () => {
                    this.GameState = GameState.Lobby;
                    this.NextButtonText = "Next question";
                },
                () => this.GameState == GameState.Results)); }
        }
        private DelegateCommand endGameCommand;

        public GameState GameState
        {
            get { return this.Game.GameState; }
            set 
            {
                if (this.Game.GameState != value)
                {
                    this.Game.GameState = value;
                    this.OnPropertyChanged(() => this.StartVisibility);
                    this.OnPropertyChanged(() => this.GameUnderwayVisibility);
                    this.OnPropertyChanged(() => this.ResultsVisibility);
                    this.OnPropertyChanged(() => this.PlayerResults);
                    this.StartGameCommand.RaiseCanExecuteChanged();
                    this.NextCommand.RaiseCanExecuteChanged();
                    this.EndGameCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Visibility StartVisibility { get { return 
            this.GameState == GameState.Lobby ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility GameUnderwayVisibility { get { return 
            this.GameState == GameState.GameUnderway ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility ResultsVisibility { get { return 
            this.GameState == GameState.Results ? Visibility.Visible : Visibility.Collapsed; } }
        public string CurrentQuestionText { get { return 
            this.Game.CurrentQuestion == null ? String.Empty : this.Game.CurrentQuestion.Text; } }

        public string NextButtonText 
        {
            get { return this.nextButtonText; }
            set { this.SetProperty(ref this.nextButtonText, value); }
        }
        private string nextButtonText;

        public List<object> PlayerProgress
        {
            get 
            { 
                var players = this.Game.SubmittedAnswers.AsEnumerable().Select(kvp => new 
                { 
                    Name = kvp.Key, 
                    AnsweredCurrentQuestionFontWeight = 
                        this.Game.CurrentQuestion != null && 
                        kvp.Value.ContainsKey(this.Game.CurrentQuestion) &&
                        kvp.Value[this.Game.CurrentQuestion].HasValue ?
                            FontWeights.ExtraBold : FontWeights.Normal,
					AnsweredCurrentQuestionBrush =
						this.Game.CurrentQuestion != null &&
						kvp.Value.ContainsKey(this.Game.CurrentQuestion) &&
						kvp.Value[this.Game.CurrentQuestion].HasValue ?
							new SolidColorBrush(Colors.Green) : 
                            new SolidColorBrush(Colors.LightGray) 
                });
                return players.ToList<object>(); 
            }
        }

        public List<object> PlayerResults
        {
            get { return this.Game.GetResults().Select(kvp => 
                new { Name = kvp.Key, Score = kvp.Value }).ToList<object>(); }
        }

        private void OnQuestionChanged()
        {
            this.OnPropertyChanged(() => this.CurrentQuestionText);
            this.OnPropertyChanged(() => this.PlayerProgress);
        }

    }
}
