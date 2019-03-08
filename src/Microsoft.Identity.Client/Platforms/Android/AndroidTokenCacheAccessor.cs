﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Cache.Items;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Exceptions;
using Microsoft.Identity.Client.Cache.Keys;

namespace Microsoft.Identity.Client.Platforms.Android
{
    internal class AndroidTokenCacheAccessor : ITokenCacheAccessor
    {
        private const string AccessTokenSharedPreferenceName = "com.microsoft.identity.client.accessToken";
        private const string RefreshTokenSharedPreferenceName = "com.microsoft.identity.client.refreshToken";
        private const string IdTokenSharedPreferenceName = "com.microsoft.identity.client.idToken";
        private const string AccountSharedPreferenceName = "com.microsoft.identity.client.Account";

        private readonly ISharedPreferences _accessTokenSharedPreference;
        private readonly ISharedPreferences _refreshTokenSharedPreference;
        private readonly ISharedPreferences _idTokenSharedPreference;
        private readonly ISharedPreferences _accountSharedPreference;

        private readonly RequestContext _requestContext;

        public AndroidTokenCacheAccessor()
        {
            _accessTokenSharedPreference = Application.Context.GetSharedPreferences(AccessTokenSharedPreferenceName,
                    FileCreationMode.Private);
            _refreshTokenSharedPreference = Application.Context.GetSharedPreferences(RefreshTokenSharedPreferenceName,
                    FileCreationMode.Private);
            _idTokenSharedPreference = Application.Context.GetSharedPreferences(IdTokenSharedPreferenceName,
                    FileCreationMode.Private);
            _accountSharedPreference = Application.Context.GetSharedPreferences(AccountSharedPreferenceName,
                FileCreationMode.Private);

            if (_accessTokenSharedPreference == null || _refreshTokenSharedPreference == null
                || _idTokenSharedPreference == null || _accountSharedPreference == null)
            {
                throw MsalExceptionFactory.GetClientException(
                    MsalError.FailedToCreateSharedPreference,
                    "Fail to create SharedPreference");
            }
        }

        public AndroidTokenCacheAccessor(RequestContext requestContext) : this()
        {
            _requestContext = requestContext;
        }

        public void SaveAccessToken(MsalAccessTokenCacheItem item)
        {
            ISharedPreferencesEditor editor = _accessTokenSharedPreference.Edit();
            editor.PutString(item.GetKey().ToString(), item.ToJsonString());
            editor.Apply();
        }

        public void SaveRefreshToken(MsalRefreshTokenCacheItem item)
        {
            ISharedPreferencesEditor editor = _refreshTokenSharedPreference.Edit();
            editor.PutString(item.GetKey().ToString(), item.ToJsonString());
            editor.Apply();
        }

        public void SaveIdToken(MsalIdTokenCacheItem item)
        {
            ISharedPreferencesEditor editor = _idTokenSharedPreference.Edit();
            editor.PutString(item.GetKey().ToString(), item.ToJsonString());
            editor.Apply();
        }

        public void SaveAccount(MsalAccountCacheItem item)
        {
            ISharedPreferencesEditor editor = _accountSharedPreference.Edit();
            editor.PutString(item.GetKey().ToString(), item.ToJsonString());
            editor.Apply();
        }

        public void DeleteAccessToken(MsalAccessTokenCacheKey cacheKey)
        {
            Delete(cacheKey.ToString(), _accessTokenSharedPreference.Edit());
        }

        public void DeleteRefreshToken(MsalRefreshTokenCacheKey cacheKey)
        {
            Delete(cacheKey.ToString(), _refreshTokenSharedPreference.Edit());
        }

        public void DeleteIdToken(MsalIdTokenCacheKey cacheKey)
        {
            Delete(cacheKey.ToString(), _idTokenSharedPreference.Edit());
        }

        public void DeleteAccount(MsalAccountCacheKey cacheKey)
        {
            Delete(cacheKey.ToString(), _accountSharedPreference.Edit());
        }

        private void Delete(string key, ISharedPreferencesEditor editor)
        {
            editor.Remove(key);
            editor.Apply();
        }

        private void DeleteAll(ISharedPreferences sharedPreferences)
        {
            var editor = sharedPreferences.Edit();

            editor.Clear();
            editor.Apply();
        }

        public IEnumerable<MsalAccessTokenCacheItem> GetAllAccessTokens()
        {
            return _accessTokenSharedPreference.All.Values.Cast<string>().Select(x => MsalAccessTokenCacheItem.FromJsonString(x)).ToList();
        }

        public IEnumerable<MsalRefreshTokenCacheItem> GetAllRefreshTokens()
        {
            return _refreshTokenSharedPreference.All.Values.Cast<string>().Select(x => MsalRefreshTokenCacheItem.FromJsonString(x)).ToList();
        }

        public IEnumerable<MsalIdTokenCacheItem> GetAllIdTokens()
        {
            return _idTokenSharedPreference.All.Values.Cast<string>().Select(x => MsalIdTokenCacheItem.FromJsonString(x)).ToList();
        }

        public IEnumerable<MsalAccountCacheItem> GetAllAccounts()
        {
            return _accountSharedPreference.All.Values.Cast<string>().Select(x => MsalAccountCacheItem.FromJsonString(x)).ToList();
        }

        public void Clear()
        {
            DeleteAll(_accessTokenSharedPreference);
            DeleteAll(_refreshTokenSharedPreference);
            DeleteAll(_idTokenSharedPreference);
            DeleteAll(_accountSharedPreference);
        }

        public MsalAccessTokenCacheItem GetAccessToken(MsalAccessTokenCacheKey accessTokenKey)
        {
            return MsalAccessTokenCacheItem.FromJsonString(_accessTokenSharedPreference.GetString(accessTokenKey.ToString(), null));
        }

        public MsalRefreshTokenCacheItem GetRefreshToken(MsalRefreshTokenCacheKey refreshTokenKey)
        {
            return MsalRefreshTokenCacheItem.FromJsonString(_refreshTokenSharedPreference.GetString(refreshTokenKey.ToString(), null));
        }

        public MsalIdTokenCacheItem GetIdToken(MsalIdTokenCacheKey idTokenKey)
        {
            return MsalIdTokenCacheItem.FromJsonString(_idTokenSharedPreference.GetString(idTokenKey.ToString(), null));
        }

        public MsalAccountCacheItem GetAccount(MsalAccountCacheKey accountKey)
        {
            return MsalAccountCacheItem.FromJsonString(_accountSharedPreference.GetString(accountKey.ToString(), null));
        }
    }
}
