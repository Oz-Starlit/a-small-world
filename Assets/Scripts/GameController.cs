﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public Camera cam;

    public GameObject world;
    public GameObject characterPrefab;

    public CameraController cameraController;

    public float gravity = 9.81f;
    public float rotationSpeed = 2f;
    public float rotationStopTime = 0.5f;
    private float currentRotationSpeed = 0;
    private float rotationVelocity = 0;
    private int alive;

    void Start() {
        Physics2D.gravity = Vector2.down * gravity;
        SpawnCharacter();
    }

    void Update() {
        if(Input.GetButtonDown("Fire2") || Input.GetButtonDown("Jump")) {
            SpawnCharacter();
        }
        UpdateWorldRotationSpeed();
    }

    void FixedUpdate() {
        ApplyRotationSpeed();
    }

    private void UpdateWorldRotationSpeed() {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition); 
        float rotation = 0;
        if(Input.GetButton("Fire1")) {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");
            float x = mouseWorldPos.x;
            float y = mouseWorldPos.y;

            rotation += x > 0 ? v : -v;
            rotation += y > 0 ? -h : h;
            rotation = Mathf.Clamp(rotation, -rotationSpeed, rotationSpeed);
        } else if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            float h = rotationSpeed * Input.GetAxis("Horizontal");
            float v = rotationSpeed * Input.GetAxis("Vertical");
            rotation = v - h;
            rotation = Mathf.Clamp(rotation, -rotationSpeed, rotationSpeed);
        } else if(currentRotationSpeed != 0) {
            rotation = 0;
        }

        if(currentRotationSpeed != rotation) {
            currentRotationSpeed = Mathf.SmoothDamp(currentRotationSpeed, rotation, ref rotationVelocity, rotationStopTime);    
        }
    }

    private void ApplyRotationSpeed() {
        if(currentRotationSpeed != 0) {
            cameraController.transform.Rotate(0, 0, currentRotationSpeed);    
            Quaternion cameraRotation = cameraController.transform.rotation;
            Physics2D.gravity = Trigonometry.DegreeToVector2(cameraRotation.eulerAngles.z + 90f) * -gravity;
        }
    }

    private void SpawnCharacter() {
        alive++;
        GameObject go = Instantiate(characterPrefab);
        go.GetComponent<Character>().gameController = this;
    }

    public void CloneCharacter(GameObject character) {
        alive++;
        Instantiate(character);
    }

    public void OnKill() {
        alive--;
        if(alive <= 0) {
            CameraFade.StartAlphaFade(Color.black, false, 1f, 1f, () => {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
    }
}
