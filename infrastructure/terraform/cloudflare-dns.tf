resource "cloudflare_record" "app" {
  zone_id = var.cloudflare_zone_id
  name    = var.app_subdomain
  content = cloudflare_zero_trust_tunnel_cloudflared.app.cname
  type    = "CNAME"
  proxied = true
  comment = "Price Tracker Application - Managed by Terraform"
}

resource "cloudflare_record" "management" {
  zone_id = var.cloudflare_zone_id
  name    = var.management_subdomain
  content = cloudflare_zero_trust_tunnel_cloudflared.management.cname
  type    = "CNAME"
  proxied = true
  comment = "Price Tracker Management - Managed by Terraform"
}
