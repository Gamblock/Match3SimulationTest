using Features.Data;
using Features.Signals;
using Features.Views;
using GamePlay.Signals;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace GamePlay.Presenters
{
    public class InputPresenter : MonoBehaviour
    {
        private TileView currentGemUnderInput;
        private TileView selectedTile;
        private SignalBus signalBus;
        private Camera cameraMain;
        
        [Inject]
        public void Inject( SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        private void Start()
        {
            cameraMain = Camera.main;
        }

        void Update()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                currentGemUnderInput = null;
                var gemUnderMouse = GetGemUnderMouse();
                if (gemUnderMouse != null)
                {
                    currentGemUnderInput = gemUnderMouse;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                var gem = GetGemUnderMouse();
                
                if (currentGemUnderInput == gem)
                {
                    // Same gem as mouse down
                    if (gem != null)
                    {
                        if (gem != selectedTile)
                            SelectTile(gem);
                        else
                            Deselect();
                    }
                }
                else
                {
                    if(currentGemUnderInput != null)
                        CheckSwipe(currentGemUnderInput);
                }

                currentGemUnderInput = null;
            }

        }

        private void CheckSwipe(TileView fromTile)
        {
            var touchPosWorld = cameraMain.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
            
            Vector2 swipe = new Vector2(touchPosWorld2D.x - fromTile.transform.position.x, touchPosWorld2D.y - fromTile.transform.position.y);
            if (swipe.magnitude < 0.05f)
                return;
            
            var nextGemCoord = fromTile.GetBoardPosition();
           
            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            {
                if (swipe.x > 0)
                    nextGemCoord.x += 1;
                else {
                    nextGemCoord.x -= 1;
                }
            }
            else
            {
                if (swipe.y > 0)
                    nextGemCoord.y += 1;
                else {
                    nextGemCoord.y -= 1;
                }
            }
            
            StartSwap(fromTile.GetBoardPosition(), nextGemCoord);
        }

        private void Deselect()
        {
            if(selectedTile != null)
                selectedTile.Deselect();
            selectedTile = null;
            
        }

        private void Select(TileView tile)
        {
            Deselect();
            selectedTile = tile;
            selectedTile.Select();
        }
        
        private void SelectTile(TileView tile)
        {
            if (selectedTile == null)
            {
                Select(tile);
            }
            else
            {
                var tilePosition = tile.GetBoardPosition();
                var selectedTilePosition = selectedTile.GetBoardPosition();
                if (BoardCalculator.IsNextToEachOther(tilePosition, selectedTilePosition))
                {
                    StartSwap(tilePosition, selectedTilePosition);
                }
                else
                {
                    Select(tile);
                }
            }
        }

        private TileView GetGemUnderMouse()
        {
           return GetTileUnderPoint(cameraMain.ScreenToWorldPoint(Input.mousePosition)); 
        }

        private TileView GetTileUnderPoint(Vector3 touchPosWorld)
        {
            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, cameraMain.transform.forward);
            if (hitInformation.collider != null) {
                GameObject touchedObject = hitInformation.transform.gameObject;
                var gemView = touchedObject.GetComponent<TileView>();
                if (gemView != null)
                {
                    return gemView;
                }
            }

            return null;
        }

        private void StartSwap(int2 tile1, int2 tile2)
        {
            signalBus.Fire(new Match3Signals.StartSwapSignal(tile1, tile2));
            Deselect();
            currentGemUnderInput = null;
        }
    }
}