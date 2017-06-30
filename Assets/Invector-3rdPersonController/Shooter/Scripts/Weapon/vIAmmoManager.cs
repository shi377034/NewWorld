
public interface vIAmmoManager
{
	bool CanReload(vShooterWeapon weapon);
	void ReloadWeapon(vShooterWeapon weapon);
}
public interface vIAmmo
{
	int id {get;}
	int count{get;set;}
}