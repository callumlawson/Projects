using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using System;

namespace HauntedHouseContentPipeline
{
    // Each tile has a texture, source rect, and sprite effects.
    [ContentSerializerRuntimeType("HauntedHouse.Tile, HauntedHouse")]
    public class HauntedHouseMapTileContent
    {
        public String TileSource;
        //public ExternalReference<Texture2DContent> Texture;
        public Rectangle SourceRectangle;
        public SpriteEffects SpriteEffects;
        public bool IsShadowCaster;
        public bool Exists;
        public Vector2 Position;
        public Vector2 GridSize;
    }

    [ContentSerializerRuntimeType("HauntedHouse.Entity, HauntedHouse")]
    public class HauntedHouseMapObjectContent
    {
        public String EntityType;
        public String EntityName;
        public Rectangle EntityBounds;
        public Dictionary<String, String> Properties;
    }

    // For each layer, we store the size of the layer and the tiles.
    [ContentSerializerRuntimeType("HauntedHouse.TileLayer, HauntedHouse")]
    public class HauntedHouseTileLayerContent
    {
        public String LayerName;
        public int Width;
        public int Height;
        public HauntedHouseMapTileContent[] Tiles;
    }

    [ContentSerializerRuntimeType("HauntedHouse.EntityLayer, HauntedHouse")]
    public class HauntedHouseObjectLayerContent
    {
        public String LayerName;
        public int Width;
        public int Height;
        public HauntedHouseMapObjectContent[] Objects;
    }

    // For the map itself, we just store the size, tile size, and a list of layers.
    [ContentSerializerRuntimeType("HauntedHouse.Level, HauntedHouse")]
    public class HauntedHouseMapContent
    {
        public int TileWidth;
        public int TileHeight;
        public List<HauntedHouseObjectLayerContent> ObjectLayers = new List<HauntedHouseObjectLayerContent>();
        public List<HauntedHouseTileLayerContent> TileLayers = new List<HauntedHouseTileLayerContent>();
    }

    [ContentProcessor(DisplayName = "TMX Processor - HauntedHouse")]
    public class MapProcessor : ContentProcessor<MapContent, HauntedHouseMapContent>
    {
        public override HauntedHouseMapContent Process(MapContent input, ContentProcessorContext context)
        {
            // build the textures
            //TiledHelpers.BuildTileSetTextures(input, context);

            // generate source rectangles
            TiledHelpers.GenerateTileSourceRectangles(input);

            // now build our output, first by just copying over some data
            HauntedHouseMapContent output = new HauntedHouseMapContent
            {
                TileWidth = input.TileWidth,
                TileHeight = input.TileHeight,
            };

            // iterate all the layers of the input
            foreach (LayerContent layer in input.Layers)
            {
                // we only care about tile layers in our demo
                
                TileLayerContent tileLayerContent = layer as TileLayerContent;
                MapObjectLayerContent mapObjectLayerContent = layer as MapObjectLayerContent;

                if (tileLayerContent != null)
                {
                    // create the new layer
                    HauntedHouseTileLayerContent outLayer = new HauntedHouseTileLayerContent
                    {
                        Width = tileLayerContent.Width,
                        Height = tileLayerContent.Height,
                        LayerName = tileLayerContent.Name,
                    };

                    // we need to build up our tile list now
                    outLayer.Tiles = new HauntedHouseMapTileContent[tileLayerContent.Data.Length];
                    for (int i = 0; i < tileLayerContent.Data.Length; i++)
                    {
                        // get the ID of the tile
                        uint tileID = tileLayerContent.Data[i];

                        // use that to get the actual index as well as the SpriteEffects
                        int tileIndex;
                        SpriteEffects spriteEffects;
                        TiledHelpers.DecodeTileID(tileID, out tileIndex, out spriteEffects);

                        // figure out which tile set has this tile index in it and grab
                        // the texture reference and source rectangle.
                        ExternalReference<Texture2DContent> textureContent = null;
                        Rectangle sourceRect = new Rectangle();
                        bool isShadowCaster = new bool();
                        bool exists = new bool();
                        String imageSource = "NotDefined";
                        exists = true;

                        // iterate all the tile sets
                        foreach (var tileSet in input.TileSets)
                        {
                            if (tileIndex != 0)
                            {
                                // if our tile index is in this set
                                if (tileIndex - tileSet.FirstId < tileSet.Tiles.Count)
                                {
                                    imageSource = tileSet.Image;
                                    // store the texture content and source rectangle
                                    textureContent = tileSet.Texture;
                                    sourceRect = tileSet.Tiles[(int)(tileIndex - tileSet.FirstId)].Source;
                                    if (tileSet.Tiles[(int)(tileIndex - tileSet.FirstId)].Properties.ContainsKey("IsShadowCaster"))
                                    {
                                        isShadowCaster = Convert.ToBoolean(tileSet.Tiles[(int)(tileIndex - tileSet.FirstId)].Properties["IsShadowCaster"]);
                                    }
                                    // and break out of the foreach loop
                                    break;
                                }
                            }
                            else
                            {
                                exists = false;
                            }
                        }

                        // now insert the tile into our output
                        outLayer.Tiles[i] = new HauntedHouseMapTileContent
                        {
                            TileSource = imageSource,
                            Exists = exists,
                            //Texture = textureContent,
                            SourceRectangle = sourceRect,
                            SpriteEffects = spriteEffects,
                            IsShadowCaster = isShadowCaster,
                            GridSize = new Vector2(output.TileWidth, output.TileHeight),
                            Position = new Vector2((float)i % tileLayerContent.Width * output.TileWidth, (float)((Math.Floor((double)i / tileLayerContent.Width) * output.TileHeight)))
                        };
                    }
                    // add the layer to our output
                    output.TileLayers.Add(outLayer);
                }

               
                if (mapObjectLayerContent != null)
                {
                    // create the new layer
                    HauntedHouseObjectLayerContent outLayer = new HauntedHouseObjectLayerContent
                    {
                        Width = mapObjectLayerContent.Width,
                        Height = mapObjectLayerContent.Height,
                        LayerName = mapObjectLayerContent.Name,
                    };

                    //Variables
                    Dictionary<String,String> properties = null;
                    Rectangle bounds = new Rectangle();
                    String name = null;
                    String type = null;

                    // we need to build up our tile list now
                    outLayer.Objects = new HauntedHouseMapObjectContent[mapObjectLayerContent.Objects.Count];
                    MapObjectContent[] mapLayerObjects = mapObjectLayerContent.Objects.ToArray();
                    for (int i = 0; i < outLayer.Objects.Length; i++)
                    {
                        if (mapLayerObjects[i].Properties != null)
                        {
                            properties = new Dictionary<string, string>(mapLayerObjects[i].Properties);
                        }
                        bounds = mapLayerObjects[i].Bounds;
                        name = mapLayerObjects[i].Name;
                        type = mapLayerObjects[i].Type;

                        // now insert the tile into our output
                        outLayer.Objects[i] = new HauntedHouseMapObjectContent
                        {
                          Properties = properties,
                          EntityName =  name,
                          EntityType = type,
                          EntityBounds = bounds
                        };
                    }

                    output.ObjectLayers.Add(outLayer);
                    }
 
                }
            
            // return the output object. because we have ContentSerializerRuntimeType attributes on our
            // objects, we don't need a ContentTypeWriter and can just use the automatic serialization.
            return output;
        }
    }
}
