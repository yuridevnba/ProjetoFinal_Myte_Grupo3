# ProjetoFinal_Myte_Grupo3

Descrição breve do projeto.

## Índice

- [Sobre](#sobre)
- [Requisitos do Projeto] (#Requisitos do Projeto)
- [Tecnologias Utilizadas](#Tecnologias Utilizadas)
- [Instalação](#instalação)
- [Autores](#Autores)

## Sobre
O Sistema de Lançamento de Horas é uma aplicação baseada no Myte projetada para facilitar o registro e a gestão de horas trabalhadas por funcionários. Este sistema oferece diversas funcionalidades que atendem a diferentes níveis de usuários, incluindo administradores e funcionários.

Funcionalidades Principais

Registro de Horas: Permite que os funcionários registrem suas horas de trabalho de forma simples e eficiente com sua WBS expecificada.

Dados de Usuário: Permite que os usuários do programa gerenciem suas informações pessoais em uma aba dedicada.

Gerenciamento de Usuários: Administradores podem adicionar, editar e remover usuários, bem como definir suas permissões e níveis de acesso.

Gerenciamento de WBS: Administradores podem adicionar, editar e remover WBS.

Relatórios de Horas: Gera relatórios detalhados das horas trabalhadas por funcionário, departamento ou projeto.





## Requisitos do Projeto

O Sistema MYTE ("My Time Entries") foi desenvolvido para registrar horas trabalhadas em atividades específicas (WBS - Work Breakdown Structure). Os principais requisitos incluem:

### Funcionalidades

- *Gerenciamento de Departamentos:*
  - Criar, recuperar, atualizar e excluir registros de departamentos.
  - Adicionar e listar departamentos, com filtragem por nome ou ID.

- *Gerenciamento de Funcionários:*
  - Criar, recuperar, atualizar e excluir registros de funcionários.
  - Cada registro contém id, nome, departamento e data de contratação.

- *Login:*
  - Permitir que usuários façam login para acessar as funcionalidades do sistema.

- *Criação e Manutenção de WBS:*
  - Criar, editar e excluir WBS.
  - Visualizar lista de WBS, com validações de código e descrição.

- *Registro de Horas:*
  - Permitir que funcionários registrem horas trabalhadas em atividades específicas.
  - Interface com linhas representando WBS e colunas representando dias da quinzena.
  - Validação de horas mínimas por dia útil e visualização do total de horas.

- *Navegação entre Quinzenas:*
  - Permitir navegação entre quinzenas para registrar horas de períodos anteriores ou futuros.

- *Relatórios:*
  - Relatórios no Power BI com análise das WBS com maior número de horas registradas.
  - Visualização gráfica e filtragem por período de tempo.

### Fluxos Principais

1. *Login:* Autenticação de usuários com redirecionamento após sucesso.
2. *Gerenciamento de WBS:* CRUD de WBS com validações de código e descrição.
3. *Registro de Horas:* Interface intuitiva para registro de horas, com validações.
4. *Navegação entre Quinzenas:* Navegação para quinzenas anteriores e futuras.
5. *Relatórios:* Geração de relatórios detalhados no Power BI.


## Tecnologias Utilizadas

### Linguagens de Programação

<div align="center">

| ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) |
|---|

</div>

### Ferramentas e Tecnologias

<div align="center">

| ![ASP.NET MVC](https://img.shields.io/badge/ASP.NET_MVC-5C2D91?style=for-the-badge&logo=dot-net&logoColor=white) | ![Identity](https://img.shields.io/badge/Identity-5C2D91?style=for-the-badge&logo=dot-net&logoColor=white) | ![Visual Studio](https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visual-studio&logoColor=white) |
|---|---|---|

</div>

