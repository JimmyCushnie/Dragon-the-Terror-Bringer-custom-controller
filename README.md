# Dragon the Terror Bringer custom controller

This is the code I wrote to run the dragon AI in my april fools 2018 joke [The Ultimate Nerd Game 2](https://www.youtube.com/watch?v=AcUf52K_U18). It controls the dragon from the excellent [Dragon the Terror Bringer](https://www.assetstore.unity3d.com/en/#!/content/77121) free Unity asset.

Please note that I wrote this whole thing in like an hour and didn't refactor it at all. It is really, really, really, really bad code and I'm sorry. But if you just want to get some dragons up and flying around then maybe it'll be useful to you.

### How to use

Add to your scene one of the dragon prefabs from the asset. Then, add the DragonController component from this repo to it; it should be attached to the same GameObject as a playerControl component from the asset.

You'll now have to set the properties of the dragon's behavior in the inspector. Here's what I'm using for most of the dragons in TUNG 2:

| Property                           | Recommended Value |
|------------------------------------|-------------------|
| Walk Speed                         | 2                 |
| Run Speed                          | 8                 |
| Fly Forward Speed                  | 15                |
| Fly Down Speed                     | 17                |
| Start State                        | Idle              |
| Height To Start Landing At         | 10                |
| Distance Beyond Which To Come Home | 200               |
| Home Spread                        | 30                |
| Min Up Angle                       | -55               |
| Max Up Angle                       | -15               |
| Min Down Angle                     | 10                |
| Max Down Angle                     | 45                |

Note that there are a lot of properties about the dragon's behavior - for example, how high it can get before it absolutely must fly down and can no longer fly up anymore - which are not configurable.
