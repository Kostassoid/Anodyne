// Copyright 2011-2013 Anodyne.
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

namespace Kostassoid.Anodyne.MongoDb
{
	using System;
	using Abstractions.DataAccess;

	public static class DataAccessProviderSelectorEx
	{
		public static DataAccessTargetSelector UseMongoDatabase(this DataAccessProviderSelector dataAccessProviderSelector, string connectionString, string databaseName)
		{
			return dataAccessProviderSelector.Use(new MongoDataProvider(connectionString, databaseName));
		}

		public static DataAccessTargetSelector UseMongoDatabase(this DataAccessProviderSelector dataAccessProviderSelector, string connectionString)
		{
			return dataAccessProviderSelector.Use(new MongoDataProvider(connectionString));
		}

		public static DataAccessTargetSelector UseMongoDatabase(this DataAccessProviderSelector dataAccessProviderSelector, Tuple<string, string> connectionStringAndDatabaseName)
		{
			return dataAccessProviderSelector.Use(new MongoDataProvider(connectionStringAndDatabaseName));
		}
	}
}