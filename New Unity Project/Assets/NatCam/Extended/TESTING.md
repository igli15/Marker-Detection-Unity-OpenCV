# NatCam Extended 1.5f3 Tests
Since NatCam 1.5f3, every release candidate must pass the NatCam Test Suite. Here is the guide:
- :heavy_check_mark: Test passed.
- :heavy_multiplication_x: Test failed. This should not occur for a release build.
- :heavy_minus_sign: Test is not applicable on platform.

Below is a matrix of test case results for this build:

| Test | iOS | Android | Legacy | Notes |
|:----:|:---:|:-------:|:------:|:------|
| FaceCam* | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No face support on legacy |
| QRCam* | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Save Photo | | | | |
| Text Reader | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | Experimental feature. Only fully supported on Android. |

## Notes
Tests marked with an asterik (*) are included in NatCam. All others can be found in the 
[NatCam Test Suite](https://github.com/olokobayusuf/NatCam-Test-Suite).