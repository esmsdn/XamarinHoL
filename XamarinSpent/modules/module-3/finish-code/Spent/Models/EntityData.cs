using System;

using Microsoft.WindowsAzure.MobileServices;

namespace Spent
{
	public class EntityData
	{
		public EntityData()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }

		[CreatedAt]
		public DateTimeOffset CreatedAt { get; set; }

		[UpdatedAt]
		public DateTimeOffset UpdatedAt { get; set; }

		[Version]
		public string AzureVersion { get; set; }
	}
}