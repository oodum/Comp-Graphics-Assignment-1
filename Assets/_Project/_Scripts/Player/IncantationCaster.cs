using Utility;
using IncantationSystem;
namespace Combat {
    public interface IIncantationCaster {
        public ObservableList<Incantation> Incantations { get; set; }
        public void Cast(float timing);
    }
}
