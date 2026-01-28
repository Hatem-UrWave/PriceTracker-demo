resource "hcloud_server" "app" {
  name        = "price-tracker-app"
  server_type = var.server_type  # cx23
  location    = var.server_location
  image       = "ubuntu-24.04"

  ssh_keys = [hcloud_ssh_key.default.id]

  labels = {
    project = var.hetzner_project_name
    role    = "application"
  }

  firewall_ids = [hcloud_firewall.app_server.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = true
  }

  user_data = <<-EOF
    #!/bin/bash
    set -e

    # Update system
    apt-get update
    apt-get upgrade -y

    # Install Docker
    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh
    rm get-docker.sh

    # Install Docker Compose
    apt-get install -y docker-compose-plugin

    # Create application directory
    mkdir -p /opt/price-tracker

    # Enable and start Docker
    systemctl enable docker
    systemctl start docker

    # Add useful aliases
    echo 'alias dc="docker compose"' >> /root/.bashrc
    echo 'alias dps="docker ps"' >> /root/.bashrc
    echo 'alias dlogs="docker compose logs -f"' >> /root/.bashrc

    echo "Application server setup complete"
  EOF
}

resource "hcloud_server" "management" {
  name        = "price-tracker-mgmt"
  server_type = var.server_type  # cx23
  location    = var.server_location
  image       = "ubuntu-24.04"

  ssh_keys = [hcloud_ssh_key.default.id]

  labels = {
    project = var.hetzner_project_name
    role    = "management"
  }

  firewall_ids = [hcloud_firewall.management_server.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = true
  }

  user_data = <<-EOF
    #!/bin/bash
    set -e

    # Update system
    apt-get update
    apt-get upgrade -y

    # Install Docker
    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh
    rm get-docker.sh

    # Install Docker Compose
    apt-get install -y docker-compose-plugin

    # Create management directory
    mkdir -p /opt/management

    # Enable and start Docker
    systemctl enable docker
    systemctl start docker

    # Add useful aliases
    echo 'alias dc="docker compose"' >> /root/.bashrc
    echo 'alias dps="docker ps"' >> /root/.bashrc
    echo 'alias dlogs="docker compose logs -f"' >> /root/.bashrc

    echo "Management server setup complete"
  EOF
}

resource "hcloud_server_network" "app" {
  server_id  = hcloud_server.app.id
  network_id = hcloud_network.main.id
  ip         = "10.0.1.10"
}

resource "hcloud_server_network" "management" {
  server_id  = hcloud_server.management.id
  network_id = hcloud_network.main.id
  ip         = "10.0.1.20"
}
