﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Management.BackupServices;
using Microsoft.Azure.Management.BackupServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.AzureBackup.Cmdlets.DataSource
{
    // ToDo:
    // Get Tracking API from Piyush and Get JobResponse
    /// <summary>
    /// Disable Azure Backup protection
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "AzureBackupProtection"), OutputType(typeof(Guid))]
    public class DisableAzureBackupProtection : AzureBackupDSCmdletBase
    {
        [Parameter(Position = 1, Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.RemoveProtectionOption)]
        public SwitchParameter RemoveRecoveryPoints 
        {
            get { return DeleteBackupData; }
            set { DeleteBackupData = value; } 
        }
        private bool DeleteBackupData;

        [Parameter(Position = 2, Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.Reason)]
        public string Reason { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.Comments)]
        public string Comments { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            ExecutionBlock(() =>
            {
                WriteVerbose("Making client call");
                RemoveProtectionRequestInput input = new RemoveProtectionRequestInput()
                {
                    RemoveProtectionOption = this.DeleteBackupData ? RemoveProtectionOptions.DeleteBackupData.ToString() : RemoveProtectionOptions.RetainBackupData.ToString(),
                    Reason = this.Reason,
                    Comments = this.Comments,
                };

                WriteVerbose("RemoveProtectionOption is = " + input.RemoveProtectionOption);
                Guid jobId = Guid.Empty;
                var disbaleAzureBackupProtection = AzureBackupClient.DataSource.DisableProtectionAsync(GetCustomRequestHeaders(), Item.ContainerUniqueName, Item.Type, Item.DataSourceId, input, CmdletCancellationToken).Result;

                WriteVerbose("Received disable azure backup protection response");
                WriteVerbose("Converting response");
                jobId = disbaleAzureBackupProtection.OperationId;
                this.WriteObject(jobId);
            });
        }
        public enum RemoveProtectionOptions
        {
            [EnumMember]
            DeleteBackupData,

            [EnumMember]
            RetainBackupData
        };
    }
}
