﻿using UnityEngine;
using System;

namespace ActionCode2D.Renderers
{
    [RequireComponent(typeof(SpriteRenderer))]
	public sealed class SpriteGhostTrailRenderer : MonoBehaviour 
	{
        private float _currentTime = 0f;
        private Transform _ghostContainer;
        private int _ghostIndex = 0;
        private SpriteRenderer[] _ghostRenderers;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        public Color colorClosest = Color.white * 0.5f;
        public Color colorFarthest = Color.white * 0.5f;
        public bool enableOnAwake = true;
        public bool fauxTransparency = true;
        [Range(1, 10)] public int ghosts = 4;
        public bool shareSprite = true;
        public bool singleColorShader = true;
        [Range(0.016f, 1f)] public float updateInterval = 0.1f;

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake () 
		{
            InitializeGhosts();
            enabled = enableOnAwake;
        }

        private void Update() {
            _currentTime += Utils.cappedDeltaTime;
            if (!(_currentTime > updateInterval)) return;
            _currentTime = 0f;

            _ghostIndex = (_ghostIndex + 1) % _ghostRenderers.Length;
            _ghostRenderers[_ghostIndex].gameObject.SetActive(true);
            PlaceGhost(_ghostRenderers[_ghostIndex]);
            UpdateGhostSprite(_ghostRenderers[_ghostIndex]);

            for (var i = 0; i < _ghostRenderers.Length; i++) {
                var ghost = _ghostRenderers[(i  + _ghostIndex + 1) % _ghostRenderers.Length];
                UpdateGhostColor(
                    ghost,
                    i / (float)(_ghostRenderers.Length - 1)
                );
                if (shareSprite) UpdateGhostSprite(ghost);
            }

            // for (int i = 0; i < _ghostRenderers.Length; i++) {
            //     SpriteRenderer ghost = _ghostRenderers[(i  + _ghostIndex + 1) % _ghostRenderers.Length];
            //     UpdateFauxTransparency(
            //         ghost,
            //         Mathf.Lerp(
            //             colorFarthest.a,
            //             colorClosest.a,
            //             i / (float)(_ghostRenderers.Length - 1)
            //         )
            //     );
            // }
        }

        private void OnEnable()
        {
            _ghostContainer.parent = null;
            foreach (var ghost in _ghostRenderers)
            {
                ghost.gameObject.SetActive(true);
            }
        }

        private void OnDisable() {
            if (!gameObject.activeInHierarchy) return;

            _currentTime = 0f;
            _ghostIndex = 0;

            _ghostContainer.parent = transform;
            foreach (var ghost in _ghostRenderers) {
                ghost.gameObject.SetActive(false);
                PlaceGhost(ghost);
            }
        }

        private void InitializeGhosts()
        {
            _ghostRenderers = new SpriteRenderer[ghosts];
            if (_ghostRenderers.Length == 0) return;

            Material material = null;
            if (singleColorShader)
            {
                var textShader = Shader.Find("GUI/Text Shader");
                if (textShader == null) Debug.LogError("GUI/Text Shader not found.");
                else material = new Material(textShader);
            }
            else
            {
                material = _spriteRenderer.material;
            }

            _ghostContainer = new GameObject(gameObject.name + "-Ghosts").transform;
            _ghostContainer.parent = transform;
            _ghostContainer.localPosition = Vector3.zero;
            _ghostContainer.localRotation = Quaternion.identity;

            GameObject baseGhost = new GameObject("Ghost-0");
            baseGhost.transform.parent = _ghostContainer;
            baseGhost.transform.localPosition = Vector3.zero;
            baseGhost.transform.localRotation = Quaternion.identity;
            baseGhost.SetActive(false);

            _ghostRenderers[0] = baseGhost.AddComponent<SpriteRenderer>();
            _ghostRenderers[0].material = material;
            _ghostRenderers[0].sprite = _spriteRenderer.sprite;
            _ghostRenderers[0].sortingLayerID = _spriteRenderer.sortingLayerID;
            // _ghostRenderers[0].sortingOrder = _spriteRenderer.sortingOrder - 1;

            for (var i = 1; i < _ghostRenderers.Length; i++) {
                var ghost = Instantiate<GameObject>(baseGhost, _ghostContainer);
                ghost.name = "Ghost-" + i;
                _ghostRenderers[i] = ghost.GetComponent<SpriteRenderer>();
            }
        }

        private void PlaceGhost(SpriteRenderer ghost)
        {
            var transform1 = ghost.transform;
            var parent = transform1.parent;
            var parentTemp = parent;
            parent = null;
            var transform2 = transform;
            transform1.localScale = transform2.lossyScale;
            parent = parentTemp;
            transform1.parent = parent;

            transform1.position = transform2.position + (Vector3.forward * 0.01F);
            transform1.rotation = transform2.rotation;
        }

        private void UpdateGhostSprite(SpriteRenderer ghost) {
            ghost.flipX = _spriteRenderer.flipX;
            ghost.flipY = _spriteRenderer.flipY;
            ghost.sprite = _spriteRenderer.sprite;
        }

        private void UpdateGhostColor(SpriteRenderer ghost, float lerpAmt) {
            ghost.color = Color.Lerp(
                colorFarthest,
                colorClosest,
                lerpAmt
            );
        }

        // private void UpdateFauxTransparency(SpriteRenderer ghost, float alpha) {
        //     Tuple<int, int> frameCounts = Utils.CalculateFauxTransparencyFrameCount(alpha);
        //     Debug.Log(alpha);
        //     Debug.Log(frameCounts);

        //     int frames = (int)Time.time * 60;
        //     frames %= frameCounts.Item1 + frameCounts.Item2;

        //     Color color = ghost.color;
        //     if (frames < frameCounts.Item1) color.a = 1;
        //     else color.a = 0;
        //     ghost.color = color;
        // }
    }
}