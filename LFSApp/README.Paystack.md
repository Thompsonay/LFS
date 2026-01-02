Paystack developer/test config
=============================

This small README explains how the project reads Paystack keys for local development and recommendations for production.

Development (temporary / local testing)
-------------------------------------

This project loads Paystack settings from configuration (default: appsettings.json). For local development the project will use values from `appsettings.Development.json` when the environment is Development.

In this workspace we added the Paystack test keys to `appsettings.Development.json` for convenience while you test locally. These are test keys and should NOT be used in production.

Recommended safer alternative (preferred): dotnet user-secrets
-------------------------------------------------------------

1. Open a PowerShell terminal in the `LFSApp` project directory.
2. Initialize user-secrets once for the project (if not already):

```powershell
dotnet user-secrets init
```

3. Set the keys (replace values with your test keys when testing):

```powershell
dotnet user-secrets set "Paystack:SecretKey" "sk_test_your_secret_here"
dotnet user-secrets set "Paystack:PublicKey" "pk_test_your_public_here"
```

Environment variables (CI / production)
-------------------------------------

On CI or production systems prefer environment variables or a secret manager:

PowerShell (session only):
```powershell
$env:Paystack__SecretKey = "sk_test_your_secret_here"
$env:Paystack__PublicKey = "pk_test_your_public_here"
dotnet run
```

Notes
-----
- Never put secret keys (sk_*) into version control in production.
- The public key (pk_*) can be embedded on the frontend, the secret key must remain server-side only.
- For production, use your production keys (sk_live_ / pk_live_) and a managed secrets store (Azure Key Vault, AWS Secrets Manager, etc.)
