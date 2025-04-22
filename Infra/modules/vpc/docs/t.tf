module "vpc1" {
  source             = "../modules/vpc"
  availability_zones = ["us-east-1a", "us-east-1b"]
  flowlog_arn        = module.iam.flowlog_arn
  settings = {
    main = {
      name   = "vpc01"
      cidr   = "172.22.0.0/16"
      region = "us-east-1"
    }
    us-east-1a = {
      cidr_public  = "172.22.0.0/24"
      cidr_private = "172.22.10.0/24"
      cidr_data    = "172.22.20.0/24"
    }
    us-east-1b = {
      cidr_public  = "172.22.1.0/24"
      cidr_private = "172.22.11.0/24"
      cidr_data    = "172.22.21.0/24"
    }
  }
  tags = {
    environment = var.environment
    owner       = var.owner
    managed-by  = "terraform"
  }
}
