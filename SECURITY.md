# Security policy

## Supported versions

Security fixes are considered for the current `main` package channel. `develop` is a preview channel and may receive a fix before promotion. Older commits and locally modified copies are not guaranteed to receive backports.

## Report a vulnerability privately

Use [GitHub private vulnerability reporting](https://github.com/Deucarian/Logging/security/advisories/new). Do not open a public issue for a suspected vulnerability.

Include the affected package version or commit, Unity version, impact, configured sink/category context, a minimal reproduction, and known mitigations. Redact credentials, personal data, endpoints, and proprietary log content.

The maintainers will triage the report in GitHub's private advisory, may ask for additional evidence, and will coordinate disclosure after a fix or mitigation is available. No response or remediation deadline is guaranteed.

Security scope includes data exposure, unsafe formatting or forwarding, sink behavior, and the editor tooling shipped by this package. Unity and separately resolved dependencies retain their own security processes.
