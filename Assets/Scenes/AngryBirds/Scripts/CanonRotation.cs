using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ICanonActions
{
    private Vector2 mousePosition;
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float projectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    private InputSystem_Actions inputActions;
    [SerializeField] private float _multiplier = 10f;


    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;

        inputActions = new InputSystem_Actions();
        inputActions.Canon.SetCallbacks(this);
	}
	public void OnEnable()
	{
        inputActions.Enable();
	}

	public void OnDisable()
	{
        inputActions.Disable();
	}

	void Update()
    {
        Vector2 mousePos = mousePosition; //obtenir el valor del click del cursor (Fer amb new input system)
        _distanceBetweenMouseAndPlayer = mousePos - (Vector2)transform.position; //obtenir el vector distància entre el canó i el cursor
        var ang = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * 180f / Mathf.PI + _offset);
		var clamp = Mathf.Clamp(ang, _minRotation.z, _maxRotation.z);
		transform.rotation = Quaternion.Euler(0,0,clamp); //en quin dels tres eixos va l'angle?


        if (isRaising)
        {
            projectileSpeed = Time.deltaTime * _multiplier + projectileSpeed; //acotar entre dos valors (mirar variables)
            projectileSpeed = Mathf.Clamp(projectileSpeed, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, projectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
        }
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, transform.position, Quaternion.identity); //canviar la posició on s'instancia
            projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(projectileSpeed, _multiplier);
            projectileSpeed = 0f;
            isRaising = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
		}
    }
}
