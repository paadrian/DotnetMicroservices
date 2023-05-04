using System;
using System.Runtime;
using Amazon.MediaConvert;
using Amazon.MediaConvert.Model;

namespace MediaConvertNET
{
    /* -\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-
     * Permissions IAM user needs to run this example
     * -\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-
     * 
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Sid": "VisualEditor0",
                    "Effect": "Allow",
                    "Action": [
                        "mediaconvert:DescribeEndpoints",
                        "mediaconvert:CreateJob"
                    ],
                    "Resource": "*"
                }
            ]
        }
    */
    /* -\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-
     * JSON job settings used in this example
     * -\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-\-
     * 
        {
          "Queue": "arn:aws:mediaconvert:us-west-2:505474453218:queues/Default",
          "UserMetadata": {
            "Customer": "Amazon"
          },
          "Role": "Your AWS Elemental MediaConvert role ARN",
          "Settings": {
            "OutputGroups": [
              {
                "Name": "File Group",
                "OutputGroupSettings": {
                  "Type": "FILE_GROUP_SETTINGS",
                  "FileGroupSettings": {
                    "Destination": "s3://youroutputdestination"
                  }
                },
                "Outputs": [
                  {
                    "VideoDescription": {
                      "ScalingBehavior": "DEFAULT",
                      "TimecodeInsertion": "DISABLED",
                      "AntiAlias": "ENABLED",
                      "Sharpness": 50,
                      "CodecSettings": {
                        "Codec": "H_264",
                        "H264Settings": {
                          "InterlaceMode": "PROGRESSIVE",
                          "NumberReferenceFrames": 3,
                          "Syntax": "DEFAULT",
                          "Softness": 0,
                          "GopClosedCadence": 1,
                          "GopSize": 90,
                          "Slices": 1,
                          "GopBReference": "DISABLED",
                          "SlowPal": "DISABLED",
                          "SpatialAdaptiveQuantization": "ENABLED",
                          "TemporalAdaptiveQuantization": "ENABLED",
                          "FlickerAdaptiveQuantization": "DISABLED",
                          "EntropyEncoding": "CABAC",
                          "Bitrate": 5000000,
                          "FramerateControl": "SPECIFIED",
                          "RateControlMode": "CBR",
                          "CodecProfile": "MAIN",
                          "Telecine": "NONE",
                          "MinIInterval": 0,
                          "AdaptiveQuantization": "HIGH",
                          "CodecLevel": "AUTO",
                          "FieldEncoding": "PAFF",
                          "SceneChangeDetect": "ENABLED",
                          "QualityTuningLevel": "SINGLE_PASS",
                          "FramerateConversionAlgorithm": "DUPLICATE_DROP",
                          "UnregisteredSeiTimecode": "DISABLED",
                          "GopSizeUnits": "FRAMES",
                          "ParControl": "SPECIFIED",
                          "NumberBFramesBetweenReferenceFrames": 2,
                          "RepeatPps": "DISABLED",
                          "FramerateNumerator": 30,
                          "FramerateDenominator": 1,
                          "ParNumerator": 1,
                          "ParDenominator": 1
                        }
                      },
                      "AfdSignaling": "NONE",
                      "DropFrameTimecode": "ENABLED",
                      "RespondToAfd": "NONE",
                      "ColorMetadata": "INSERT"
                    },
                    "AudioDescriptions": [
                      {
                        "AudioTypeControl": "FOLLOW_INPUT",
                        "CodecSettings": {
                          "Codec": "AAC",
                          "AacSettings": {
                            "AudioDescriptionBroadcasterMix": "NORMAL",
                            "RateControlMode": "CBR",
                            "CodecProfile": "LC",
                            "CodingMode": "CODING_MODE_2_0",
                            "RawFormat": "NONE",
                            "SampleRate": 48000,
                            "Specification": "MPEG4",
                            "Bitrate": 64000
                          }
                        },
                        "LanguageCodeControl": "FOLLOW_INPUT",
                        "AudioSourceName": "Audio Selector 1"
                      }
                    ],
                    "ContainerSettings": {
                      "Container": "MP4",
                      "Mp4Settings": {
                        "CslgAtom": "INCLUDE",
                        "FreeSpaceBox": "EXCLUDE",
                        "MoovPlacement": "PROGRESSIVE_DOWNLOAD"
                      }
                    },
                    "NameModifier": "_1"
                  }
                ]
              }
            ],
            "AdAvailOffset": 0,
            "Inputs": [
              {
                "AudioSelectors": {
                  "Audio Selector 1": {
                    "Offset": 0,
                    "DefaultSelection": "NOT_DEFAULT",
                    "ProgramSelection": 1,
                    "SelectorType": "TRACK",
                    "Tracks": [
                      1
                    ]
                  }
                },
                "VideoSelector": {
                  "ColorSpace": "FOLLOW"
                },
                "FilterEnable": "AUTO",
                "PsiControl": "USE_PSI",
                "FilterStrength": 0,
                "DeblockFilter": "DISABLED",
                "DenoiseFilter": "DISABLED",
                "TimecodeSource": "EMBEDDED",
                "FileInput": "s3://yourinputfile"
              }
            ],
            "TimecodeConfig": {
              "Source": "EMBEDDED"
            }
          }
        }
    */

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var mediaConvertRole = "arn:aws:iam::172326106741:role/service-role/MediaConvert_Default_Role";
            var fileInput = "s3://paadrian-dev-bucket/Michael_Jackson_Billie_Jean.mp3";
            var fileOutput = "s3://paadrian-dev-bucket/Michael_Jackson_Billie_Jean";
            // Once you know what your customer endpoint is, set it here
            var mediaConvertEndpoint = "";

            // If we do not have our customer-specific endpoint
            if (string.IsNullOrEmpty(mediaConvertEndpoint))
            {
                // Obtain the customer-specific MediaConvert endpoint
                mediaConvertEndpoint = await GetEndpoint(mediaConvertEndpoint);
            }

            // Since we have a service url for MediaConvert, we do not
            // need to set RegionEndpoint. If we do, the ServiceURL will
            // be overwritten
            var mcConfig = new AmazonMediaConvertConfig
            {
                ServiceURL = mediaConvertEndpoint,
            };

            var mcClient = new AmazonMediaConvertClient(mcConfig);
            var createJobRequest = GetJobRequest(mediaConvertRole, fileInput, fileOutput);

            try
            {
                var createJobResponse = await mcClient.CreateJobAsync(createJobRequest);
                Console.WriteLine("Job Id: {0}", createJobResponse.Job.Id);
            }
            catch (BadRequestException bre)
            {
                Console.WriteLine(bre.Message);
                // If the enpoint was bad
                if (bre.Message.StartsWith("You must use the customer-"))
                {
                    // The exception contains the correct endpoint; extract it
                    mediaConvertEndpoint = bre.Message.Split('\'')[1];
                    // Code to retry query
                }
            }
        }

        private static CreateJobRequest GetJobRequest(string mediaConvertRole, string fileInput, string fileOutput)
        {
            var createJobRequest = new CreateJobRequest();

            createJobRequest.Role = mediaConvertRole;
            createJobRequest.UserMetadata.Add("Customer", "Amazon");

            createJobRequest.Settings = GetJobSettings();

            var ofg = GetOutputGroup(fileOutput);
            createJobRequest.Settings.OutputGroups.Add(ofg);

            var input = GetInput(fileInput);
            createJobRequest.Settings.Inputs.Add(input);

            return createJobRequest;
        }

        private static Input GetInput(string fileInput)
        {
            var input = new Input();
            input.FilterEnable = InputFilterEnable.AUTO;
            input.PsiControl = InputPsiControl.USE_PSI;
            input.FilterStrength = 0;
            input.DeblockFilter = InputDeblockFilter.DISABLED;
            input.DenoiseFilter = InputDenoiseFilter.DISABLED;
            input.TimecodeSource = InputTimecodeSource.EMBEDDED;
            input.FileInput = fileInput;
            var audsel = GetAudioSelector();
            input.AudioSelectors.Add("Audio Selector 1", audsel);

            input.VideoSelector = new VideoSelector();
            input.VideoSelector.ColorSpace = ColorSpace.FOLLOW;
            return input;
        }

        private static AudioSelector GetAudioSelector()
        {
            var audsel = new AudioSelector();
            audsel.Offset = 0;
            audsel.DefaultSelection = AudioDefaultSelection.NOT_DEFAULT;
            audsel.ProgramSelection = 1;
            audsel.SelectorType = AudioSelectorType.TRACK;
            audsel.Tracks.Add(1);
            return audsel;
        }

        private static OutputGroup GetOutputGroup(string fileOutput)
        {
            OutputGroup ofg = new OutputGroup();
            ofg.Name = "File Group";
            ofg.OutputGroupSettings = new OutputGroupSettings();
            ofg.OutputGroupSettings.Type = OutputGroupType.FILE_GROUP_SETTINGS;
            ofg.OutputGroupSettings.FileGroupSettings = new FileGroupSettings();
            ofg.OutputGroupSettings.FileGroupSettings.Destination = fileOutput;
            var output = GetOutput();

            ofg.Outputs.Add(output);
            return ofg;
        }

        private static Output GetOutput()
        {
            Output output = new Output();
            output.NameModifier = "_1";


            var ades = GetAudioDescriptor();
            output.AudioDescriptions.Add(ades);

            SetupMp4(output);
            return output;
        }

        private static AudioDescription GetAudioDescriptor()
        {
            AudioDescription ades = new AudioDescription();
            ades.LanguageCodeControl = AudioLanguageCodeControl.FOLLOW_INPUT;
            // This name matches one specified in the Inputs below
            ades.AudioSourceName = "Audio Selector 1";
            ades.CodecSettings = new AudioCodecSettings();
            ades.CodecSettings.Codec = AudioCodec.AAC;
            AacSettings aac = new AacSettings();
            aac.AudioDescriptionBroadcasterMix = AacAudioDescriptionBroadcasterMix.NORMAL;
            aac.RateControlMode = AacRateControlMode.CBR;
            aac.CodecProfile = AacCodecProfile.LC;
            aac.CodingMode = AacCodingMode.CODING_MODE_2_0;
            aac.RawFormat = AacRawFormat.NONE;
            aac.SampleRate = 48000;
            aac.Specification = AacSpecification.MPEG4;
            aac.Bitrate = 64000;
            ades.CodecSettings.AacSettings = aac;
            return ades;
        }

        private static void SetupMp4(Output output)
        {
            output.ContainerSettings = new ContainerSettings();
            output.ContainerSettings.Container = ContainerType.MP4;
            Mp4Settings mp4 = new Mp4Settings();
            mp4.CslgAtom = Mp4CslgAtom.INCLUDE;
            mp4.FreeSpaceBox = Mp4FreeSpaceBox.EXCLUDE;
            mp4.MoovPlacement = Mp4MoovPlacement.PROGRESSIVE_DOWNLOAD;
            output.ContainerSettings.Mp4Settings = mp4;
        }

        private static JobSettings GetJobSettings()
        {
            var jobSettings = new JobSettings();
            jobSettings.AdAvailOffset = 0;
            jobSettings.TimecodeConfig = new TimecodeConfig();
            jobSettings.TimecodeConfig.Source = TimecodeSource.EMBEDDED;
            return jobSettings;
        }

        private static async Task<string> GetEndpoint(string mediaConvertEndpoint)
        {
            AmazonMediaConvertClient client = new AmazonMediaConvertClient(Amazon.RegionEndpoint.USEast1);
            DescribeEndpointsRequest describeRequest = new DescribeEndpointsRequest();
            DescribeEndpointsResponse describeResponse = await client.DescribeEndpointsAsync(describeRequest);
            mediaConvertEndpoint = describeResponse.Endpoints[0].Url;
            return mediaConvertEndpoint;
        }

        private static void Foo()
        {

        }
    }
}