# Fingerprint Attendance — Firebase environment

Starter configuration for a fingerprint attendance system with Arduino devices and a C# desktop app. When copying this setup into the existing project at `E:\\fingertrack`, keep `fingertrack.slnx` and its `fingertrack` subfolder in place; add these files beside them.

```text
E:\\fingertrack\\
├── fingertrack.slnx
├── fingertrack\\             # Existing Visual Studio C# project
├── arduino\\                 # Arduino/ESP32 firmware
├── docs\\
├── firebase.json
├── firestore.rules
└── .firebaserc
```

## Firebase project

- Project ID: `fndvmiltkpzaazeuqikq`
- Firestore and Firebase Authentication: reported as enabled
- Firebase CLI target: `default` (see `.firebaserc`)

## Local development

Prerequisites: Node.js LTS, Firebase CLI, and Java 21+ for the local emulators.

```powershell
npm install -g firebase-tools
firebase login
firebase emulators:start --only auth,firestore
```

The Auth emulator runs at `http://127.0.0.1:9099`, Firestore at `http://127.0.0.1:8080`, and the Emulator UI at `http://127.0.0.1:4000`.

The emulator does not require production credentials. Use test accounts created in the Auth emulator and keep development data separate from the live project.

## Security model

- Firestore rules deny access by default.
- A signed-in user can read their own user profile and attendance events.
- Admin access requires the trusted `admin` custom claim. Never let a client set its own role.
- Attendance event writes are intentionally closed to clients. A trusted backend must validate a device and write events using the Admin SDK.
- Never put service account files, private keys, or admin credentials in this repository, Arduino firmware, or the desktop app.

See [docs/architecture.md](docs/architecture.md) for the integration boundary and proposed data shape.

## Before deploying rules

Review the rules with the project owner and confirm the intended roles and data model. Deploying rules affects the live project:

```powershell
firebase deploy --only firestore:rules --project fndvmiltkpzaazeuqikq
```

Do not run this deployment until the team has reviewed the rules and agreed to the access model. This repository only prepares local configuration; it does not change Firebase Console settings or IAM.
