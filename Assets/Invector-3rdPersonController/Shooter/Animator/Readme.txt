If your Player or AI is not causing any damage on the melee attack, is because Unity lost reference of the vMeleeAttackBehaviour script on your Attack Animation States.

Just replace the your current animator and don't forget the .meta files for the ones on the AnimatorFix.rar to go back to the default settings.