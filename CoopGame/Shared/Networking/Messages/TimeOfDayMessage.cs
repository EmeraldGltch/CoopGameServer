using CoopGame.Shared.Networking;

namespace CoopGame.Shared.Networking.Messages;

public class TimeOfDayMessage : IMessage {
	public float timeOfDay;
	public int dayCount;
}
