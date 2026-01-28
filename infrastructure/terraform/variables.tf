variable "hetzner_api_token" {
  description = "Hetzner Cloud API Token"
  type        = string
  sensitive   = true
}

variable "hetzner_project_name" {
  description = "Hetzner Project Name"
  type        = string
  default     = "Price Tracker"
}

variable "cloudflare_api_token" {
  description = "Cloudflare API Token"
  type        = string
  sensitive   = true
}

variable "cloudflare_account_id" {
  description = "Cloudflare Account ID"
  type        = string
}

variable "cloudflare_zone_id" {
  description = "Cloudflare Zone ID for urwave.dev"
  type        = string
}

variable "domain" {
  description = "Base domain"
  type        = string
  default     = "urwave.dev"
}

variable "app_subdomain" {
  description = "Subdomain for the application"
  type        = string
  default     = "tracker"
}

variable "management_subdomain" {
  description = "Subdomain for management tools"
  type        = string
  default     = "mgmt"
}

variable "server_location" {
  description = "Hetzner datacenter location"
  type        = string
  default     = "nbg1"
}

variable "server_type" {
  description = "Hetzner server type"
  type        = string
  default     = "cx23"  # Updated to cx23
}

variable "ssh_public_key" {
  description = "SSH public key for server access"
  type        = string
}

variable "admin_email" {
  description = "Admin email for Cloudflare Access"
  type        = string
}
