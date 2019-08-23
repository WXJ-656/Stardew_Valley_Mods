using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Utilities;
using StardewValley;
using SObject = StardewValley.Object;
namespace Revitalize.Framework.Hacks
{
    public class ObjectInteractionHacks
    {

        /// <summary>
        /// Returns the object underneath the mouse's position.
        /// </summary>
        /// <returns></returns>
        ///
        public static Dictionary<GameLocation, List<SObject>> TrackedMachines = new Dictionary<GameLocation, List<SObject>>();
        public static SObject GetItemAtMouseTile()
        {
            if (Game1.player == null) return null;
            if (Game1.player.currentLocation == null) return null;
            Vector2 mouseTilePosition = Game1.currentCursorTile;
            if (Game1.player.currentLocation.isObjectAtTile((int)mouseTilePosition.X, (int)mouseTilePosition.Y))
            {
                return Game1.player.currentLocation.getObjectAtTile((int)mouseTilePosition.X, (int)mouseTilePosition.Y);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Checks for right mouse button iteraction with the world.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Input_CheckForObjectInteraction(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if(e.Button== StardewModdingAPI.SButton.MouseRight)
            {
                SObject obj= GetItemAtMouseTile();
                if (obj == null) return;
                if (ObjectUtilities.IsObjectFurnace(obj) && ObjectUtilities.IsObjectHoldingItem(obj)==false)
                {
                    bool crafted=VanillaRecipeBook.VanillaRecipes.TryToCraftRecipe(obj);
                    if (crafted == false) return;
                    obj.initializeLightSource((Vector2)(obj.TileLocation), false);
                    obj.showNextIndex.Value = true;
                    /*
                    Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(30, this.tileLocation.Value * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float)(((double)this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0)
                    {
                        alphaFade = 0.005f
                    });
                    */
                    (Game1.player.currentLocation).TemporarySprites.Add(new TemporaryAnimatedSprite(30, obj.TileLocation * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float)(((double)obj.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0));
                    obj.addWorkingAnimation(Game1.player.currentLocation);
                    if (TrackedMachines.ContainsKey(Game1.player.currentLocation))
                    {
                        TrackedMachines[Game1.player.currentLocation].Add(obj);
                    }
                    else
                    {
                        TrackedMachines.Add(Game1.player.currentLocation, new List<SObject>()
                    {
                        obj
                    });
                    }
                }
            }
        }

        public static void Render_RenderCustomObjectsHeldInMachines(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (TrackedMachines.ContainsKey(Game1.player.currentLocation))
            {
                List<SObject> removalList = new List<SObject>();
                foreach(SObject obj in TrackedMachines[Game1.player.currentLocation])
                {
                    if (obj.heldObject.Value == null)
                    {
                        removalList.Add(obj);
                    }
                    else
                    {
                        if(obj.heldObject.Value is CustomObject)
                        {
                            if (obj.MinutesUntilReady == 0)
                            {
                                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                                Vector2 pos = new Vector2(obj.TileLocation.X * Game1.tileSize, (obj.TileLocation.Y-1) * Game1.tileSize - 32+num);
                                obj.heldObject.Value.draw(e.SpriteBatch, (int)pos.X, (int)pos.Y, 0.25f, 1f);
                            }
                        }
                    }
                }
                foreach(SObject obj in removalList)
                {
                    TrackedMachines[Game1.player.currentLocation].Remove(obj);
                }
            }
            else
            {

            }
        }

    }
}