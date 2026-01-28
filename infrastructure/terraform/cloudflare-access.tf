resource "cloudflare_zero_trust_access_application" "management" {
  account_id                = var.cloudflare_account_id
  name                      = "Price Tracker Management"
  domain                    = "${var.management_subdomain}.${var.domain}"
  type                      = "self_hosted"
  session_duration          = "24h"
  auto_redirect_to_identity = false

  policies = [
    cloudflare_zero_trust_access_policy.management_policy.id
  ]
}

resource "cloudflare_zero_trust_access_policy" "management_policy" {
  account_id = var.cloudflare_account_id
  name       = "Management Access Policy"
  decision   = "allow"

  include {
    email = [var.admin_email]
  }
}

# Note: Path-based access protection is configured at the application level
# For Hangfire dashboard protection, use Nginx basic auth or configure
# Cloudflare Access at the main domain level if needed
