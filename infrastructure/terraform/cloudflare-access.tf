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

# Optional: Protect Hangfire dashboard with Access
resource "cloudflare_zero_trust_access_application" "hangfire" {
  account_id                = var.cloudflare_account_id
  name                      = "Hangfire Dashboard"
  domain                    = "${var.app_subdomain}.${var.domain}"
  path                      = "/hangfire"
  type                      = "self_hosted"
  session_duration          = "24h"
  auto_redirect_to_identity = false

  policies = [
    cloudflare_zero_trust_access_policy.hangfire_policy.id
  ]
}

resource "cloudflare_zero_trust_access_policy" "hangfire_policy" {
  account_id = var.cloudflare_account_id
  name       = "Hangfire Access Policy"
  decision   = "allow"

  include {
    email = [var.admin_email]
  }
}
