resource "cloudflare_zero_trust_tunnel_cloudflared" "app" {
  account_id = var.cloudflare_account_id
  name       = "price-tracker-app"
  secret     = random_id.tunnel_secret_app.b64_std
}

resource "cloudflare_zero_trust_tunnel_cloudflared" "management" {
  account_id = var.cloudflare_account_id
  name       = "price-tracker-management"
  secret     = random_id.tunnel_secret_management.b64_std
}

resource "random_id" "tunnel_secret_app" {
  byte_length = 32
}

resource "random_id" "tunnel_secret_management" {
  byte_length = 32
}

resource "cloudflare_zero_trust_tunnel_cloudflared_config" "app" {
  account_id = var.cloudflare_account_id
  tunnel_id  = cloudflare_zero_trust_tunnel_cloudflared.app.id

  config {
    ingress_rule {
      hostname = "${var.app_subdomain}.${var.domain}"
      service  = "http://nginx:80"
    }
    ingress_rule {
      service = "http_status:404"
    }
  }
}

resource "cloudflare_zero_trust_tunnel_cloudflared_config" "management" {
  account_id = var.cloudflare_account_id
  tunnel_id  = cloudflare_zero_trust_tunnel_cloudflared.management.id

  config {
    ingress_rule {
      hostname = "${var.management_subdomain}.${var.domain}"
      service  = "http://localhost:80"
    }
    ingress_rule {
      service = "http_status:404"
    }
  }
}
