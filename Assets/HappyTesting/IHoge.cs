using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace HappyTesting {
    public interface IHoge {
        string Hoge { get; }
        void SetHoge(string hoge);
        IObservable<Unit> HogeObsevable { get; }
        IReadOnlyReactiveProperty<int> HogeValue { get; }
        UniTask SetFugaPiyoAsync(string fuga, float piyo,  CancellationToken cancellationToken);
        UniTask<(string, float)> GetFugaPiyoAsync(CancellationToken cancellationToken);
    }
}
