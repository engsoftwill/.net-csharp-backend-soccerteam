using System;
using System.Collections.Generic;
using Codenation.Challenge.Exceptions;
using System.Linq;

namespace Codenation.Challenge
{
    public class SoccerTeamsManager : IManageSoccerTeams
    {
        private List<Player> Players = new List<Player>();  //instancio a lista Players
        private List<Team>  Teams =  new List<Team>();      //instancio a lista Teams
        public SoccerTeamsManager()                         
        {
        }
        public bool lookforplayers(long ID)                 //método permite verificar se existe o jogador com o ID(id do jogador que desejo testar)
        {
            return Players.Any(x => x.Id.Equals(ID));       //Any verifica os atributos que atendem a condição informada, no caso ID dentre os jogadores 
        }
        public bool lookforteams(long IDTEAM)               //método análogo ao lookforplayers, mas com foco nos times
        {
            return Teams.Any(x => x.Id.Equals(IDTEAM));
        }
        public void AddTeam(long id, string name, DateTime createDate, string mainShirtColor, string secondaryShirtColor)
        {
            if (lookforteams(id))       //testa se time já foi informado
            {
                throw new UniqueIdentifierException("Already informed Team");
            }
            Teams.Add(new Team          //adiciona novo time
            {
                DataCriacao             =   createDate,
                CorUniformePrincipal    =   mainShirtColor,
                CorUniformeSecundario   =   secondaryShirtColor,
                Id                      =   id,
                Name                    =   name
            });
        }

        public void AddPlayer(long id, long teamId, string name, DateTime birthDate, int skillLevel, decimal salary)
        {
            if (lookforplayers(id))     //testa se jogador ja existe
            {
                throw new UniqueIdentifierException("Already informed Player");
            }
            if (!lookforteams(teamId))  //testa se é um time valido, pois o time deve existir para que o jogador seja adicionado no time
            {
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            }
            Players.Add(new Player      //adiciona jogador
            {
                BirthDate   =   birthDate,
                SkillLevel  =   skillLevel,
                Salary      =   salary,
                Id          =   id,
                Name        =   name,
                TeamId      =   teamId
            });
        }
        public void SetCaptain(long playerId)   //Atributos necessários para adicionar capitao (idtimecap novo capitao) (verficar se idtimecap ja possui capantigo)
        {
            if (!lookforplayers(playerId))
            {
                throw new PlayerNotFoundException("Player not listed yet please check the id again");
            }
            var newcap      =   Players.FindIndex(x => x.Id == playerId);
            var teamidcap   =   Players[newcap].TeamId; //obtenho idnovocapitao
            var oldcap      =   Players.Where(x => x.TeamId == teamidcap)   //verifico antigo se capantigo existe
            .Where(x=> x.iscap == true)
            .SingleOrDefault();
            if(oldcap != null)
            {
                var oldcapId    =   Players.FindIndex(x => x.Id == (oldcap.Id));
                Players[oldcapId].iscap   =   false;
            }
            Players[newcap].iscap     =   true;                             //faco do jogador playerId novo capitao
        }
        public long GetTeamCaptain(long teamId) //verificar se o teamId possui capitao se sim retornar idcapitao
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            var cap   =   Players.Where(x    =>  x.TeamId == teamId)
            .Where(x    =>  x.iscap == true)
            .SingleOrDefault();
            if(cap == null)
                throw new CaptainNotFoundException("Captain not listed yet please check the id again");
            return cap.Id;
        }
        public string GetPlayerName(long playerId) //buscar jogador na lista de jogadores pelo playerId e exibir seu nome
        {
            if (!lookforplayers(playerId))
                throw new PlayerNotFoundException("Player not listed yet please check the id again");
            var player = Players.Where(x    =>  x.Id == playerId)
            .SingleOrDefault();
            return player.Name;
        }
        public string GetTeamName(long teamId)  //buscar na lista de times pelo teamId e exibir seu nome nome
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            var team = Teams.Where(x    =>  x.Id == teamId)
            .SingleOrDefault();
            return team.Name;
        }
        public List<long> GetTeamPlayers(long teamId) // buscar na lista de times pelo teamId e exibir nomes dos jogadores do time
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            return Players
            .Where(x => x.TeamId==teamId)
            .OrderBy(x=> x.Id)                      //ordena os jogadores por id necessário para teste
            .Select(x=> x.Id)
            .ToList();
        }

        public long GetBestTeamPlayer(long teamId)      //busca na lista de times pelo teamId e exibe melhor jogador do time
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            return Players.Where(x    =>  x.TeamId == teamId)
            .OrderByDescending(x =>x.SkillLevel)        //maiores skilllevel ficam em cima e pega o id do primeio da lista
            .First().Id;
        }
        public long GetOlderTeamPlayer(long teamId)     //busca na lista de times pelo teamId e retorna jogador mais velho do time
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            return Players.Where(x    =>  x.TeamId == teamId)
            .OrderBy(x =>x.BirthDate)                   //ordena jogadores pela data de nascimento e retorna id
            .ThenBy(x => x.Id)
            .First().Id;
        }

        public List<long> GetTeams()    //verifica os times incluidos na lista
        {
            var teams = Teams.OrderBy(x    =>  x.Id)
            .Select(x   =>  x.Id)
            .ToList();
            if(teams ==null)            //se não há times retorna uma lista vazia
                return new List<long>();
            else
                return  teams;

        }

        public long GetHigherSalaryPlayer(long teamId) //buca na lista de times pelo teamId e retorna jogador com maior salario do time
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException("Team not listed yet please check the id again");
            return Players.Where(x    =>  x.TeamId == teamId)
            .OrderByDescending(x =>x.Salary)
            .ThenBy(x => x.Id)
            .First().Id;
        }

        public decimal GetPlayerSalary(long playerId)   // busca na lista de Jogadores pelo playerId e retorna o valor de seu salário
        {
            if (!lookforplayers(playerId))
                throw new PlayerNotFoundException("Player not listed yet please check the id again");
            var jogador = Players.Where(x    =>  x.Id == playerId)
            .SingleOrDefault();
            return jogador.Salary;
        }

        public List<long> GetTopPlayers(int top)        // retorna a quantidade int top de melhores jogadores
        {
            var topplayers = Players
            .OrderByDescending(x =>x.SkillLevel)
            .ThenBy(x => x.Id)
            .Take(top)
            .Select(x    =>  x.Id)
            .ToList();
            if(topplayers == null)
                return new List<long>();                // se não há jogadores incluidos na lista retorna lista vazia
            else
                return  topplayers;

        }

        public string GetVisitorShirtColor(long teamId, long visitorTeamId) //busca na lista de times o time da casa e o visitante e seleciona as cores de uniformes
        {
            if (!lookforteams(teamId))
                throw new TeamNotFoundException();
            if (!lookforteams(visitorTeamId))
                throw new TeamNotFoundException();
            var timeA = Teams.Where(x    =>  x.Id == teamId)
            .SingleOrDefault();
            var timeB = Teams.Where(x    =>  x.Id == visitorTeamId)
            .SingleOrDefault();
            if (timeA.CorUniformePrincipal == timeB.CorUniformePrincipal) //cores das camisas precisam ser diferentes, para isso o visitante pode precisar mudar de cor da camisa
                return timeB.CorUniformeSecundario;
            else
                return timeB.CorUniformePrincipal;
        }
    }
    public class Player
    {
        public Player()
        {
        }
        public  long Id { get; set; }
        public  long TeamId { get; set; }
        public  string Name { get; set; }
        public  DateTime BirthDate { get; set; }
        public  int SkillLevel { get; set; }
        public  decimal Salary { get; set; }
        public bool iscap { get; set; }
    }
    public class Team
    {
        public Team()
        {
        } 
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime DataCriacao { get; set; }
        public string CorUniformePrincipal { get; set; }
        public string CorUniformeSecundario { get; set; }
    }
}