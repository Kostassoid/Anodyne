// Copyright 2011-2012 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
// 

namespace Kostassoid.BlogNote.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserModel
    {
        [Required]
        [StringLength(6, MinimumLength = 3)]
        [Display(Name = "User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [ScaffoldColumn(false)]
        public string Name { get; set; }

        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "EMail (optional)")]
        public string Email { get; set; }

        public UserModel(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public UserModel()
        {
            Name = "";
            Email = "";
        }
    }
}