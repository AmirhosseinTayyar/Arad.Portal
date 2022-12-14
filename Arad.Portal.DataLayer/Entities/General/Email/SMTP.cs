//
//  --------------------------------------------------------------------
//  Copyright (c) 2005-2021 Arad ITC.
//
//  Author : Ammar Heidari <ammar@arad-itc.org>
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0 
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  --------------------------------------------------------------------



using static Arad.Portal.DataLayer.Models.Shared.Enums;

namespace Arad.Portal.DataLayer.Entities.General.Email
{
    /// <summary>
    // smtp stand for simple mail transfer protocol if using this protocole for sending email fields have to be filled
    /// </summary>
    public class SMTP 
    {
        public string SMTPId { get; set; }

        public string Server { get; set; }

        public string EmailAddress { get; set; }

        public string DisplayName { get; set; }

        public EmailEncryptionType Encryption { get; set; }

        public int ServerPort { get; set; }

        public string SMTPAuthUsername { get; set; }

        public string SMTPAuthPassword { get; set; }

        public bool IgnoreSSLWarning { get; set; }

        public bool IsDefault { get; set; }
    }
}
