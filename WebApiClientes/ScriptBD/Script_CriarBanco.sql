/* Script para criar base de dados do BlazorClientes (WebApiClientes) */
CREATE DATABASE IF NOT EXISTS `myerp` 
/*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ 
/*!80016 DEFAULT ENCRYPTION='N' */;

/* Criar tabela de usuários */
CREATE TABLE IF NOT EXISTS `usuarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(45) DEFAULT NULL,
  `Email` varchar(150) NOT NULL,
  `Senha` varchar(200) NOT NULL,
  `TipoConta` varchar(45) DEFAULT NULL,
  `PrimeiroNome` varchar(45) DEFAULT NULL,
  `UltimoNome` varchar(45) DEFAULT NULL,
  `Celular` varchar(45) DEFAULT NULL,
  `Endereco` varchar(150) DEFAULT NULL,
  `Complemento` varchar(100) DEFAULT NULL,
  `CEP` varchar(10) DEFAULT NULL,
  `Bairro` varchar(45) DEFAULT NULL,
  `Cidade` varchar(45) DEFAULT NULL,
  `Pais` varchar(45) DEFAULT NULL,
  `Estado` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `Email_UNIQUE` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3;

/* Criar tabela Clientes */
CREATE TABLE IF NOT EXISTS `clientes` (
  `idCliente` varchar(120) NOT NULL,
  `Nome` varchar(75) DEFAULT NULL,
  `Endereco` varchar(120) DEFAULT NULL,
  `Telefone` varchar(15) DEFAULT NULL,
  `Celular` varchar(15) DEFAULT NULL,
  `Email` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`idCliente`),
  UNIQUE KEY `idClientes_UNIQUE` (`idCliente`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/* Criar tabela Produtos */
CREATE TABLE IF NOT EXISTS `produtos` (
  `idProduto` varchar(120) NOT NULL,
  `Produto` varchar(100) DEFAULT NULL,
  `Descricao` varchar(45) DEFAULT NULL,
  `Valor` decimal(13,2) DEFAULT NULL,
  `Barcode` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idProduto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


/* Criar tabela Pedidos */
CREATE TABLE IF NOT EXISTS `pedidos` (
  `IdPedido` varchar(120) NOT NULL,
  `idCliente` varchar(120) NOT NULL,
  `DataEmissao` datetime DEFAULT NULL,
  `DataEntrega` datetime DEFAULT NULL,
  `IdVendedor` varchar(45) NOT NULL,
  `vComissao` decimal(13,2) DEFAULT NULL,
  `ValorTotal` decimal(13,2) DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`IdPedido`),
  KEY `Clientes_FK_idx` (`idCliente`),
  KEY `Vendedores_FK_idx` (`IdVendedor`),
  CONSTRAINT `Clientes_FK` FOREIGN KEY (`idCliente`) REFERENCES `clientes` (`idCliente`),
  CONSTRAINT `Vendedores_FK` FOREIGN KEY (`IdVendedor`) REFERENCES `vendedores` (`idVendedor`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


/* Criar tabela ItensPedido */
CREATE TABLE IF NOT EXISTS `itenspedido` (
  `Indice` int NOT NULL AUTO_INCREMENT,
  `idPedido` varchar(120) NOT NULL,
  `idProduto` varchar(120) NOT NULL,
  `Quantidade` int NOT NULL,
  `ValorUnitario` decimal(13,2) NOT NULL,
  `pDesconto` int NOT NULL DEFAULT '0',
  `Valor` decimal(13,2) DEFAULT NULL,
  PRIMARY KEY (`Indice`),
  KEY `Pedidos_FK_idx` (`idPedido`),
  KEY `Produtos_FK_idx` (`idProduto`),
  CONSTRAINT `Pedidos_FK` FOREIGN KEY (`idPedido`) REFERENCES `pedidos` (`IdPedido`),
  CONSTRAINT `Produtos_FK` FOREIGN KEY (`idProduto`) REFERENCES `produtos` (`idProduto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;