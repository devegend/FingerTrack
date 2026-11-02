# Arduino device firmware

Put the Arduino/ESP32 firmware for the fingerprint reader in this folder. Keep device credentials out of source control and firmware binaries.

The device should send attendance scans to a trusted API that validates the registered device and writes to Firestore. Do not connect an Arduino directly with a Firebase Admin/service-account credential. The API contract and device enrollment flow still need to be designed with the team.
