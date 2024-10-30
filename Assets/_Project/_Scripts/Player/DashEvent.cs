using EventBus;

namespace Combat {
	public struct DashEvent : IEvent {
		public PlayerMovement Movement;
		
		public DashEvent(PlayerMovement movement) {
			Movement = movement;
		}
	}
}