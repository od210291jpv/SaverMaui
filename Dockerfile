FROM raspbian/stretch
WORKDIR /usr/src/app
COPY . .
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current
RUN echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
RUN echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
EXPOSE 5000
EXPOSE 80