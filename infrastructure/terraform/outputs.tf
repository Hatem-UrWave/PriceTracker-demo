output "app_server_ip" {
  description = "IP address of the application server"
  value       = hcloud_server.app.ipv4_address
}

output "management_server_ip" {
  description = "IP address of the management server"
  value       = hcloud_server.management.ipv4_address
}

output "app_tunnel_token" {
  description = "Cloudflare Tunnel token for app server"
  value       = cloudflare_tunnel.app.tunnel_token
  sensitive   = true
}

output "management_tunnel_token" {
  description = "Cloudflare Tunnel token for management server"
  value       = cloudflare_tunnel.management.tunnel_token
  sensitive   = true
}

output "app_url" {
  description = "Application URL"
  value       = "https://${var.app_subdomain}.${var.domain}"
}

output "management_url" {
  description = "Management URL"
  value       = "https://${var.management_subdomain}.${var.domain}"
}

output "server_details" {
  description = "Server configuration details"
  value = {
    project      = var.hetzner_project_name
    server_type  = var.server_type
    location     = var.server_location
    app_server   = hcloud_server.app.name
    mgmt_server  = hcloud_server.management.name
  }
}
