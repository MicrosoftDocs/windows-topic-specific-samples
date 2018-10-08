/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

//
// Declaration of the SeZoUC class.
//

#pragma once

#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "SeZoUC.g.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    struct SeZoUC : SeZoUCT<SeZoUC>
    {
        SeZoUC();
    };
}

namespace winrt::Bookstore2Universal_10::factory_implementation
{
    struct SeZoUC : SeZoUCT<SeZoUC, implementation::SeZoUC>
    {
    };
}
