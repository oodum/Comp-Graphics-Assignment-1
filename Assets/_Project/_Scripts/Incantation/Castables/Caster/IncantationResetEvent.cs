using EventBus;
using GameEntity;

namespace IncantationSystem.Castables {
	public struct IncantationResetEvent : IEvent {
		public Entity Entity;
		public IncantationResetEvent(Entity entity) => Entity = entity;
	}
}