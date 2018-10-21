/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#pragma once

#include "Author.g.h"
#include "BookSku.h"

using namespace winrt;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

namespace winrt::Bookstore2Universal_10::implementation
{
    struct Author : AuthorT<Author, IVectorView<IInspectable>>, winrt::observable_vector_base<Author, IInspectable>
    {
        Author() = delete;
        Author(std::wstring const& name);

        winrt::hstring Name();

        Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable> BookSkus();

        auto& get_container() const noexcept
        {
            return m_values;
        }

        auto& get_container() noexcept
        {
            return m_values;
        }

        void AddBookSku(Bookstore2Universal_10::BookSku const& bookSku);
        static Bookstore2Universal_10::Author GetAuthorByName(std::wstring name);

    private:
        std::wstring m_name;
        std::vector<IInspectable> m_values;
        static std::map<std::wstring, Author const*> s_authorDictionary;
        static std::mutex s_authorDictionary_mutex;
    };
}
