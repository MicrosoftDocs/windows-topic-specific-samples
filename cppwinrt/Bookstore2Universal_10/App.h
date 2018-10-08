/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#pragma once
#include "App.xaml.g.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    struct App : AppT<App>
    {
        App();

        void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs const&);
        void OnSuspending(IInspectable const&, Windows::ApplicationModel::SuspendingEventArgs const&);
        void OnNavigationFailed(IInspectable const&, Windows::UI::Xaml::Navigation::NavigationFailedEventArgs const&);
    };
}
