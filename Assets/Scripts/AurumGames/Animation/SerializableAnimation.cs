using System;
using System.Collections.Generic;
using System.Text;
using AurumGames.Animation.Tracks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AurumGames.Animation
{
#if UNITY_EDITOR
    [Serializable]
    public class SerializableAnimation
    {
        public float TimeScale = 1;
        public SerializableTrackInfo[] Tracks;
    }

    [Serializable]
    public class SerializableTrackInfo
    {
        public string TrackType;
        public Object Target;
        public string Easing;
        public SerializableFrameInfo[] Frames;
    }

    [Serializable]
    public class SerializableFrameInfo
    {
        public int Time;
        [SerializeReference] public object Value;
        public string Easing;
    }

    public static class SerializableAnimationConverter
    {
        public static readonly Dictionary<string, TrackConversionInfo> ConversionMap = new()
        {
            { 
                nameof(AnchoredPositionTrack), 
                new TrackConversionInfo(typeof(AnchoredPositionTrack), typeof(RectTransform), typeof(Vector2)) 
            },
            { 
                nameof(AnchorMaxTrack), 
                new TrackConversionInfo(typeof(AnchorMaxTrack), typeof(RectTransform), typeof(Vector2)) 
            },
            { 
                nameof(AnchorMinTrack), 
                new TrackConversionInfo(typeof(AnchorMinTrack), typeof(RectTransform), typeof(Vector2)) 
            },
            { 
                nameof(CameraBackgroundColorTrack), 
                new TrackConversionInfo(typeof(CameraBackgroundColorTrack), typeof(Camera), typeof(Color)) 
            },
            { 
                nameof(CanvasGroupOpacityTrack), 
                new TrackConversionInfo(typeof(CanvasGroupOpacityTrack), typeof(CanvasGroup), typeof(float)) 
            },
            { 
                nameof(EulerAnglesTrack), 
                new TrackConversionInfo(typeof(EulerAnglesTrack), typeof(Transform), typeof(Vector3)) 
            },
            { 
                nameof(ImageAlphaTrack), 
                new TrackConversionInfo(typeof(ImageAlphaTrack), typeof(Image), typeof(float)) 
            },
            { 
                nameof(ImageColorTrack), 
                new TrackConversionInfo(typeof(ImageColorTrack), typeof(Image), typeof(Color)) 
            },
            { 
                nameof(LocalEulerAnglesTrack), 
                new TrackConversionInfo(typeof(LocalEulerAnglesTrack), typeof(Transform), typeof(Vector3)) 
            },
            { 
                nameof(LocalPositionTrack), 
                new TrackConversionInfo(typeof(LocalPositionTrack), typeof(Transform), typeof(Vector3)) 
            },
            { 
                nameof(LocalRotationTrack), 
                new TrackConversionInfo(typeof(LocalRotationTrack), typeof(Transform), typeof(Quaternion)) 
            },
            { 
                nameof(ScaleTrack), 
                new TrackConversionInfo(typeof(ScaleTrack), typeof(Transform), typeof(Vector3)) 
            },
            { 
                nameof(SizeDeltaTrack), 
                new TrackConversionInfo(typeof(SizeDeltaTrack), typeof(RectTransform), typeof(Vector2)) 
            },
            { 
                nameof(TextMeshProColorTrack), 
                new TrackConversionInfo(typeof(TextMeshProColorTrack), typeof(TextMeshProUGUI), typeof(float)) 
            },
            { 
                nameof(VoidTrack), 
                new TrackConversionInfo(typeof(VoidTrack), null, typeof(float)) 
            },
            {
                nameof(SpriteRendererAlphaTrack),
                new TrackConversionInfo(typeof(SpriteRendererAlphaTrack), typeof(SpriteRenderer), typeof(float))
            }
        };

        public static TracksEvaluator Convert(SerializableAnimation animation)
        {
            var tracks = new ITrack[animation.Tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                SerializableTrackInfo trackInfo = animation.Tracks[i];
                TrackConversionInfo conversionInfo = ConversionMap[trackInfo.TrackType];
                var trackEasing = Easing.Functions.TryGetValue(trackInfo.Easing, out var function) 
                    ? function 
                    : Easing.Linear;

                if (conversionInfo.TrackType == typeof(VoidTrack))
                {
                    tracks[i] = new VoidTrack((int)trackInfo.Frames[0].Value);
                }
                else
                {
                    Type keyFrameType = typeof(KeyFrame<>).MakeGenericType(conversionInfo.KeyframeType);
                    var frames = Array.CreateInstance(keyFrameType, trackInfo.Frames.Length);
                    for (int j = 0; j < frames.Length; j++)
                    {
                        SerializableFrameInfo frameInfo = trackInfo.Frames[j];
                        var easing = frameInfo.Easing == "Default" || Easing.Functions.ContainsKey(frameInfo.Easing) == false
                            ? trackEasing 
                            : Easing.Functions[frameInfo.Easing];
                        frames.SetValue(Activator.CreateInstance(keyFrameType,
                            (int)(frameInfo.Time * animation.TimeScale),
                            frameInfo.Value,
                            easing
                        ), j);
                    }

                    tracks[i] = (ITrack)Activator.CreateInstance(conversionInfo.TrackType, trackInfo.Target, frames);
                }
            }

            return new TracksEvaluator(tracks);
        }
        
        public static SerializableAnimation ApplyTimeScale(SerializableAnimation animation)
        {
            var newAnimation = new SerializableAnimation
            {
                TimeScale = animation.TimeScale,
                Tracks = new SerializableTrackInfo[animation.Tracks.Length]
            };
            for (int i = 0; i < animation.Tracks.Length; i++)
            {
                SerializableTrackInfo trackInfo = animation.Tracks[i];
                newAnimation.Tracks[i] = new SerializableTrackInfo()
                {
                    TrackType = trackInfo.TrackType,
                    Easing = trackInfo.Easing,
                    Target = trackInfo.Target,
                    Frames = new SerializableFrameInfo[trackInfo.Frames.Length]
                };
                for (int j = 0; j < trackInfo.Frames.Length; j++)
                {
                    SerializableFrameInfo frameInfo = trackInfo.Frames[j];
                    newAnimation.Tracks[i].Frames[j] = new SerializableFrameInfo()
                    {
                        Easing = frameInfo.Easing,
                        Time = frameInfo.Time,
                        Value = frameInfo.Value
                    };
                }
            }
            
            
            var tracks = new ITrack[newAnimation.Tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                SerializableTrackInfo trackInfo = newAnimation.Tracks[i];
                TrackConversionInfo conversionInfo = ConversionMap[trackInfo.TrackType];

                if (conversionInfo.TrackType == typeof(VoidTrack))
                {
                    trackInfo.Frames[0].Value = (int)((int)trackInfo.Frames[0].Value * newAnimation.TimeScale);
                }
                else
                {
                    for (int j = 0; j < trackInfo.Frames.Length; j++)
                    {
                        SerializableFrameInfo frameInfo = trackInfo.Frames[j];
                        frameInfo.Time = (int)(frameInfo.Time * newAnimation.TimeScale);
                    }
                }
            }

            newAnimation.TimeScale = 1;
            return newAnimation;
        }

        public static string ConvertToCode(SerializableAnimation animation)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("new ITrack[]\n{");

            foreach (SerializableTrackInfo trackInfo in animation.Tracks)
            {
                TrackConversionInfo conversionInfo = ConversionMap[trackInfo.TrackType];
                
                stringBuilder.AppendFormat("{0}new {1}(/* {2} */, new [] \n{0}{{\n", 
                    Intend(1),
                    trackInfo.TrackType, 
                    $"{trackInfo.Target.name}, {conversionInfo.TargetType.Name}"
                    );
                foreach (SerializableFrameInfo frameInfo in trackInfo.Frames)
                {
                    var easing = frameInfo.Easing == "Default" || Easing.Functions.ContainsKey(frameInfo.Easing) == false
                        ? trackInfo.Easing 
                        : frameInfo.Easing;
                    var easingParameter = easing != nameof(Easing.Linear) && string.IsNullOrWhiteSpace(easing) == false 
                        ? $", Easing.{easing}" 
                        : "";
                    stringBuilder.AppendFormat("{0}new KeyFrame<{1}>({2}, {3}{4}), \n", 
                        Intend(3),
                        conversionInfo.KeyframeType.Name, 
                        frameInfo.Time, 
                        ValueToStringConstructor(frameInfo.Value), 
                        easingParameter
                        );
                }
                stringBuilder.AppendFormat("{0}}}),\n", Intend(1));

            }

            stringBuilder.AppendLine("}");
            
            return stringBuilder.ToString();

            string Intend(int intend)
            {
                return new string(' ', intend * 4);
            }
            
            string ValueToStringConstructor(object value)
            {
                if (value is int valueInt)
                {
                    return valueInt.ToString();
                }
                
                if (value is float valueFloat)
                {
                    return $"{valueFloat}f";
                }

                if (value is Vector2 valueVector2)
                {
                    return $"new Vector2({valueVector2.x}f, {valueVector2.y}f)";
                }

                if (value is Vector3 valueVector3)
                {
                    return $"new Vector3({valueVector3.x}f, {valueVector3.y}f, {valueVector3.z}f)";
                }
                
                if (value is Vector4 valueVector4)
                {
                    return $"new Vector4({valueVector4.x}f, {valueVector4.y}f, {valueVector4.z}f, {valueVector4.w}f)";
                }

                if (value is Quaternion valueQuaternion)
                {
                    return $"new Quaternion({valueQuaternion.x}f, {valueQuaternion.y}f, {valueQuaternion.z}f, {valueQuaternion.w}f)";
                }

                if (value is Color valueColor)
                {
                    return $"new Color({valueColor.r}f, {valueColor.g}f, {valueColor.b}f, {valueColor.a}f)";
                }

                return $"/*Unsupported {value.GetType().Name}*/";
            }
        }
    }

    public struct TrackConversionInfo : IEquatable<TrackConversionInfo>
    {
        public bool Equals(TrackConversionInfo other)
        {
            return Equals(TrackType, other.TrackType) && Equals(TargetType, other.TargetType) && Equals(KeyframeType, other.KeyframeType);
        }

        public override bool Equals(object obj)
        {
            return obj is TrackConversionInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrackType, TargetType, KeyframeType);
        }

        public static bool operator ==(TrackConversionInfo left, TrackConversionInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TrackConversionInfo left, TrackConversionInfo right)
        {
            return !left.Equals(right);
        }

        public Type TrackType;
        public Type TargetType;
        public Type KeyframeType;

        public TrackConversionInfo(Type trackType, Type targetType, Type keyframeType)
        {
            TrackType = trackType;
            TargetType = targetType;
            KeyframeType = keyframeType;
        }
    }
#endif
}