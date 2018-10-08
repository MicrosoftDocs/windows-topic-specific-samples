/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#pragma once

#include "Author.h"
#include "BookSku.h"

namespace winrt::Bookstore2Universal_10::implementation
{
	class DataSource
	{
	public:
		static void LoadDataFromCloudService(Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>&, Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>&);
		static void LoadSampleData(Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>&, Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>&);
	};
}