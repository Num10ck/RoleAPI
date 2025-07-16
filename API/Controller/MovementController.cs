namespace RoleAPI.API.Controller
{
	using Exiled.API.Features;

	using UnityEngine;

	public class MovementController : MonoBehaviour
	{
		private Player _player;
		private Vector3 _offset;
		
		public void Init(Player player, Vector3 offset)
		{
			_player = player;
			_offset = offset;

			Log.Debug("[MovementController] Controller initialized.");
		}

		private void Update()
		{
			transform.position = _player.GameObject.transform.position + _offset;
			transform.rotation = _player.GameObject.transform.rotation;
		}
	}
}