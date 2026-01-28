resource "hcloud_ssh_key" "default" {
  name       = "price-tracker-key"
  public_key = var.ssh_public_key

  labels = {
    project = var.hetzner_project_name
  }
}
