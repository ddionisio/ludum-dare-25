using UnityEngine;
using System.Collections;

public class AISetState : SequencerAction {
	public Entity.State state = Entity.State.idle;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		ai.state = state;
	}
}
