locals {
  # Convert project name to valid Hetzner label format (lowercase, no spaces)
  project_label = lower(replace(var.hetzner_project_name, " ", "-"))
}
