using System.Collections;
using System.Collections.Generic;
using App.Config;
using DG.Tweening;
using Features.Data;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Features.Views
{
    public delegate void OnDestructionAnimationComplete(TileView view);

    public class TileView : MonoBehaviour
    {
        public event OnDestructionAnimationComplete OnDestructionAnimationComplete;
        
        [SerializeField] private float destroyDuration = 0.3f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Vector3 defaultScale;
        private int2 boardPosition;
        private TileColor tileColor;
        private AssetsCatalogue catalogue;
        
        [Inject]
        private void Inject(AssetsCatalogue catalogue)
        {
            this.catalogue = catalogue;
        }
        
        public void Init(int2 position, TileColor color)
        {
            boardPosition = position;
            tileColor = color;
            spriteRenderer.sprite = catalogue.GetSpriteConfig(tileColor);
            defaultScale = transform.localScale;
        }

        public void SetPosition(int2 position)
        {
            boardPosition = position;
        }
        public void PlayDestroyAnimation()
        {
            gameObject.transform
                .DOScale(0.1f, destroyDuration)
                .OnComplete(() =>
                {
                    gameObject.transform.localScale = defaultScale;
                    OnDestructionAnimationComplete?.Invoke(this);
                });
        }
        
        public void Select()
        {
            spriteRenderer.color = Color.gray;
        }

        public void Deselect()
        {
            spriteRenderer.color = Color.white;
        }
        
        public int2 GetBoardPosition()
        {
            return boardPosition;
        }

        public TileColor GetTileColor()
        {
            return tileColor;
        }
    }
}
