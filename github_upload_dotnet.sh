#!/bin/bash

# ==============================================================================
# SCRIPT: github_upload_dotnet.sh
# FOCO: CachyOS/Arch Linux (usa pacman)
# DESCRIÇÃO: Prepara projeto .NET (cria .gitignore se preciso), instala dependências 
#            (git, gh), inicializa o Git, cria repositório no GitHub e faz o push.
#
# USO: ./github_upload_dotnet.sh <NOME_DO_NOVO_REPOSITORIO>
# ==============================================================================

# 1. Configuração
GITHUB_USERNAME="vandermjr"
REPO_NAME="$1"

# 2. Verificação de Argumentos
if [ "$#" -ne 1 ]; then
    echo "❌ Uso incorreto."
    echo "   Uso: $0 <NOME_DO_NOVO_REPOSITORIO>"
    exit 1
fi

# -----------------------------------------------------------------------------

## VERIFICAÇÃO E INSTALAÇÃO DE DEPENDÊNCIAS (PACMAN)

echo "🛠️ Verificando e instalando dependências (foco CachyOS/Arch)..."

# 2.1. Verifica e instala o GIT CLI
if ! command -v git &> /dev/null; then
    echo "🚨 Git CLI não encontrada. Instalando..."
    sudo pacman -S --noconfirm git
    if [ $? -ne 0 ]; then
        echo "❌ Erro ao instalar o Git. O script será abortado."
        exit 1
    fi
else
    echo "✅ Git CLI já instalada."
fi

# 2.2. Verifica e instala o GITHUB CLI (gh)
if ! command -v gh &> /dev/null; then
    echo "🚨 GitHub CLI (gh) não encontrada. Instalando..."
    sudo pacman -S --noconfirm github-cli
    if [ $? -ne 0 ]; then
        echo "❌ Erro ao instalar o GitHub CLI. O script será abortado."
        exit 1
    fi
else
    echo "✅ GitHub CLI (gh) já instalada."
fi

# 2.3. Verifica a autenticação do GitHub CLI
if ! gh auth status &> /dev/null; then
    echo
    echo "⚠️ Você precisa estar logado no GitHub CLI para criar o repositório."
    echo "   Abrindo o processo de autenticação..."
    gh auth login

    if ! gh auth status &> /dev/null; then
        echo "❌ Falha na autenticação. O script será abortado."
        exit 1
    fi
    echo "✅ Autenticação realizada com sucesso!"
fi

# 2.4. Garante a existência do .gitignore do .NET
if [ ! -f .gitignore ]; then
    echo "📄 Criando arquivo .gitignore para projeto .NET..."
    dotnet new gitignore &> /dev/null
fi

echo "---"
echo "✅ Iniciando upload do projeto .NET..."
echo "✅ Repositório de destino: https://github.com/$GITHUB_USERNAME/$REPO_NAME (Público)"
echo "---"

# 3. Inicialização e Commit Local
echo "🚀 1/3: Inicializando Git e criando commit local..."

if [ ! -d .git ]; then
    git init
    git branch -M main
fi

# Adiciona todos os arquivos respeitando o .gitignore
git add .
COMMIT_MSG="feat: initial commit of Avalonia Calendar control"
git commit -m "$COMMIT_MSG"

if [ $? -ne 0 ]; then
    echo "❌ Erro ao criar o commit. Verifique se há alterações a confirmar."
    exit 1
fi

# 4. Criação e Push Remoto
echo
echo "🚀 2/3: Criando repositório remoto público ($REPO_NAME) e enviando código..."

gh repo create "$GITHUB_USERNAME/$REPO_NAME" --public --source="." --push

if [ $? -ne 0 ]; then
    echo "❌ Erro ao criar o repositório e fazer o push."
    echo "   Pode ser que o nome do repositório ($REPO_NAME) já exista na sua conta."
    exit 1
fi

# 5. Finalização
echo
echo "🎉 SUCESSO! O projeto foi enviado para o GitHub."
echo "🔗 URL do seu novo repositório: https://github.com/$GITHUB_USERNAME/$REPO_NAME"

exit 0