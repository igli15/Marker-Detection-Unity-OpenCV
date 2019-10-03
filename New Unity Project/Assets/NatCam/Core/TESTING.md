# NatCam Core 1.5f3 Tests
Since NatCam 1.5f3, every release candidate must pass the NatCam Test Suite. Here is the guide:
- :heavy_check_mark: Test passed.
- :heavy_multiplication_x: Test failed. This should not occur for a release build.
- :heavy_minus_sign: Test is not applicable on platform.

Below is a matrix of test case results for this build:

| Test | iOS | Android | Legacy | Notes |
|:----:|:---:|:-------:|:------:|:------|
| Camera Switch | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Darker Camera | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No exposure support on Legacy |
| Exposure Lock | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No exposure support on Legacy |
| Focus Fixer   | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No focus support on Legacy |
| MiniCam*      | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| NatCam Behaviour* | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Pause Play    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Release Play  | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Rotate Photo  | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | |
| Torch Light   | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No torch support on Legacy |
| ZoomCam       | :heavy_check_mark: | :heavy_check_mark: | :heavy_minus_sign: | No zoom support on Legacy |

## Notes
Tests marked with an asterik (*) are included in NatCam. All others can be found in the 
[NatCam Test Suite](https://github.com/olokobayusuf/NatCam-Test-Suite).