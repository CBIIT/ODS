resource "aws_db_subnet_group" "this" {
  name       = "${var.identifier}-subnet-group"
  subnet_ids = var.subnet_ids
  tags       = var.tags
}

resource "aws_security_group" "this" {
  name        = "${var.identifier}-sg"
  description = "Security group for ${var.identifier} Aurora PostgreSQL cluster"
  vpc_id      = var.vpc_id
  
  # Allow PostgreSQL traffic from within the VPC
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/8"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = var.tags
}

resource "aws_rds_cluster" "this" {
  cluster_identifier           = var.identifier
  engine                       = "aurora-postgresql"
  engine_version               = var.engine_version
  database_name                = "ods"
  master_username              = var.master_username
  master_password              = var.master_password
  db_subnet_group_name         = aws_db_subnet_group.this.name
  vpc_security_group_ids       = [aws_security_group.this.id]
  backup_retention_period      = var.backup_retention_period
  preferred_backup_window      = var.preferred_backup_window
  preferred_maintenance_window = var.preferred_maintenance_window
  deletion_protection          = var.deletion_protection
  storage_encrypted            = true
  
  # For serverless v2
  serverlessv2_scaling_configuration {
    min_capacity = var.serverless_min_capacity
    max_capacity = var.serverless_max_capacity
  }
  
  tags = merge(var.tags, {
    Name = var.identifier
  })
}

# Writer instance
resource "aws_rds_cluster_instance" "writer" {
  identifier             = "${var.identifier}-instance-1"
  cluster_identifier     = aws_rds_cluster.this.id
  engine                 = "aurora-postgresql"
  engine_version         = var.engine_version
  instance_class         = var.instance_class
  db_subnet_group_name   = aws_db_subnet_group.this.name
  availability_zone      = "us-west-1a" # Match the screenshot
  
  tags = merge(var.tags, {
    Name = "${var.identifier}-instance-1"
  })
}

# Reader instance
resource "aws_rds_cluster_instance" "reader" {
  identifier             = "${var.identifier}-instance-1-rds"
  cluster_identifier     = aws_rds_cluster.this.id
  engine                 = "aurora-postgresql"
  engine_version         = var.engine_version
  instance_class         = var.instance_class
  db_subnet_group_name   = aws_db_subnet_group.this.name
  availability_zone      = "us-west-1b" # Match the screenshot
  
  # This is a reader instance
  promotion_tier         = 1
  
  tags = merge(var.tags, {
    Name = "${var.identifier}-instance-1-rds"
  })
}
