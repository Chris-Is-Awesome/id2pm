namespace ModStuff.Commands
{
	public class TestCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			/*
			string roomName = SceneAndRoomHelper.GetRoomPlayerIsIn().RoomName;
			Vector3 pos = GameObject.Find("PlayerEnt").transform.position;
			Vector3 rot = GameObject.Find("PlayerEnt").transform.localEulerAngles;
			float posX = (float)Math.Round(pos.x, 2);
			float posY = (float)Math.Round(pos.y, 2);
			float posZ = (float)Math.Round(pos.z, 2);
			float rotX = (float)Math.Round(rot.x, 2);
			float rotY = (float)Math.Round(rot.y, 2);
			float rotZ = (float)Math.Round(rot.z, 2);
			return WriteDataToFile("						{ new RoomData(" + '"' + roomName + '"' + ", new Vector3(" + posX + "f, " + posY + "f, " + posZ + "f), new Vector3(" + rotX + "f, " + rotY + "f, " + rotZ + "f)) },");
			*/

			return "Nothing is being tested. Stop slacking! A modder should always be testing stuff! :mjau:";
		}

		private string WriteDataToFile(object data)
		{
			SaveManager.SaveToCustomFile(data, "test.txt", "", false);
			return "Successfully wrote test data to file!";
		}
	}
}