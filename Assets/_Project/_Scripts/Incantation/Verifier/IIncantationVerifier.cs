namespace IncantationSystem {
	public interface IIncantationVerifier {
		public bool Verify(Incantation incantation, out string error);
	}
}
