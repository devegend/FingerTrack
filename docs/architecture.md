# Integration and security notes

## Components

1. **Fingerprint reader + Arduino** captures a template match and creates a minimal attendance event. Do not store raw fingerprint images or templates in Firestore.
2. **Trusted API/backend (to be implemented)** authenticates each registered device, validates its payload, applies replay/idempotency checks, and writes the event through Firebase Admin SDK. Keep its service identity in the hosting platform's secret/IAM configuration.
3. **C# desktop app** signs users in with Firebase Authentication and uses a trusted API for administrative actions and event ingestion. The API verifies Firebase ID tokens and enforces roles. Do not ship an Admin SDK credential with the desktop application.
4. **Firebase Auth + Firestore** hold user identity, device registry, and attendance records. Firestore rules protect client reads; backend writes bypass rules and must validate authorization themselves.

Arduino firmware should not contain a service account key, Firebase Admin credential, or a shared long-lived secret. A public Firebase web API key is not an authorization secret, but it does not authenticate the device. Treat a device identifier by itself as untrusted.

## Initial Firestore shape

- `users/{uid}`: display name, status, and non-sensitive employee reference. Admin roles are Firebase custom claims, set only by a trusted administrator/backend.
- `devices/{deviceId}`: registration status and metadata. Never store reusable device secrets in a readable document.
- `attendanceEvents/{eventId}`: `userId`, `deviceId`, `eventType`, `occurredAt`, and a server-side receipt timestamp. Use an idempotency key to prevent duplicate scans.

Keep biometric templates on the fingerprint module or a separately designed protected system; attendance records should contain the matched user's ID, not biometric data.

## Before connecting real hardware

- Agree on employee enrollment, device registration/revocation, offline behavior, and the API contract.
- Provision individual device credentials securely and support rotation/revocation.
- Validate timestamps and payload sizes server-side; use server timestamps for receipt time.
- Define retention and access policies for attendance records.
- Test rule behavior with the Auth and Firestore emulators before any production deployment.

## Current scope

This repository configures the Firebase CLI target and local Auth/Firestore emulators and provides a deny-by-default Firestore rules baseline. It does not provision IAM users, deploy a backend, or configure Firebase Console providers. Those require project-owner decisions and implementation.
