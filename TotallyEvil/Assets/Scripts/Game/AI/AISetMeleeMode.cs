using UnityEngine;
using System.Collections;

public class AISetMeleeMode : SequencerAction {
	public Enemy.Melee mode = Enemy.Melee.Off;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Enemy ai = (Enemy)behaviour;
		ai.meleeMode = mode;
	}
}
