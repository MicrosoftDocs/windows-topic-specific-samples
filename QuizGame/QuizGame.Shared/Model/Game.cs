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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuizGame.Model
{
    public enum GameState { Lobby, GameUnderway, Results }

    public sealed class Game : BindableBase, QuizGame.Model.IGame
    {
        public event EventHandler<QuestionEventArgs> NewQuestionAvailable = delegate { };
        private int currentQuestionIndex = -1;

        public Game(List<Question> questions = null)
        {
            // TODO if questions == null, populate this.Questions from XML data.
            this.Questions = questions;
            this.PlayerNames = new ObservableCollection<string>();
            this.SubmittedAnswers = new Dictionary<string, Dictionary<Question, int?>>();
        }

        public void AddPlayer(string playerName)
        {
			if (this.PlayerNames.Contains(playerName)) playerName += ".";
			this.PlayerNames.Add(playerName);
            this.SubmittedAnswers.Add(playerName, 
                new Dictionary<Question, int?>(this.Questions.Count));
            this.OnPropertyChanged(() => this.PlayerNames);
            this.OnPropertyChanged(() => this.SubmittedAnswers);
        }

        public void RemovePlayer(string playerName)
        {
            if (this.PlayerNames.Remove(playerName))
            {
                // TODO confirm that this is necessary to avoid leaking the nested dictionary
                this.SubmittedAnswers[playerName] = null;

                this.SubmittedAnswers.Remove(playerName);
                this.OnPropertyChanged(() => this.PlayerNames);
                this.OnPropertyChanged(() => this.SubmittedAnswers);
            }
        }

        // TODO returning bool seems wrong since it's a test-only concern
        // throw exception instead, and requiring exception handling for post-game-end submission?
        public bool SubmitAnswer(string playerName, int answerIndex)
        {
            if (playerName == null || this.CurrentQuestion == null) return false;
            this.SubmittedAnswers[playerName][this.CurrentQuestion] = answerIndex;
            this.OnPropertyChanged(() => this.SubmittedAnswers);
            return true;
        }

        public void StartGame()
        {
            this.currentQuestionIndex = 0;
            this.SendCurrentQuestion();
        }

        public void NextQuestion() 
        { 
            this.currentQuestionIndex++;
            this.SendCurrentQuestion();
        }

        private void SendCurrentQuestion()
        {
            if (this.currentQuestionIndex < this.Questions.Count)
            {
                this.NewQuestionAvailable(this, new QuestionEventArgs { Question = this.CurrentQuestion });
            }
            this.OnPropertyChanged(() => this.CurrentQuestion);
            this.OnPropertyChanged(() => this.IsGameOver);
            this.OnPropertyChanged(() => this.Winner);
        }

        public Dictionary<string, int> GetResults()
        {
            var correctAnswers = this.Questions.Select(question => question.CorrectAnswerIndex);
            var results =
                from playerResults in SubmittedAnswers.AsEnumerable()
                let score = playerResults.Value.AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .Zip(correctAnswers, (playerAnswer, actualAnswer) =>
                        playerAnswer.HasValue && playerAnswer.Value == actualAnswer)
                    .Count(isCorrect => isCorrect)
                select new { PlayerName = playerResults.Key, Score = score };
            return results.ToDictionary(result => result.PlayerName, result => result.Score);
        }

        public GameState GameState
        {
            get { return this.gameState; }
            set { SetProperty(ref this.gameState, value); }
        }
        private GameState gameState;

        public ObservableCollection<String> PlayerNames { get; set; }
        public List<Question> Questions { get; private set; }
        public Question CurrentQuestion 
        { 
            get 
            { 
                return this.currentQuestionIndex > -1 && 
                    this.currentQuestionIndex < this.Questions.Count ? 
                    this.Questions[currentQuestionIndex] : null; 
            } 
        }

        public Dictionary<string, Dictionary<Question, int?>> SubmittedAnswers { get; private set; }
        public bool IsGameOver { get { return this.currentQuestionIndex >= this.Questions.Count; } }
        public string Winner { get { return this.IsGameOver ? 
            this.GetResults().Aggregate((a, b) => a.Value > b.Value ? a : b).Key : null; } }

    }
}
