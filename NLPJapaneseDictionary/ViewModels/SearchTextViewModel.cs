﻿/**
 * Copyright © 2017-2018 Anki Universal Team.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may
 * not use this file except in compliance with the License.  A copy of the
 * License is distributed with this work in the LICENSE.md file.  You may
 * also obtain a copy of the License from
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NLPJDict.Models;
using NLPJDict.NLPJDictCore.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPJDict.ViewModels
{
    public class SearchTextViewModel
    {
        public ObservableCollectionAutoResize<SearchTextModel> SearchedTexts { get; set; }

        public SearchTextViewModel(int maxSize)
        {
            SearchedTexts = new ObservableCollectionAutoResize<SearchTextModel>(maxSize);
        }

        public void AddFirstNonDuplicate(SearchTextModel model)
        {
            if (SearchedTexts.Count == 0 || !SearchedTexts[0].Text.Equals(model.Text, StringComparison.OrdinalIgnoreCase))
            {
                SearchedTexts.Insert(0, model);
            }
        }

        public void Remove(SearchTextModel model)
        {
            SearchedTexts.Remove(model);
        }
    }
}
